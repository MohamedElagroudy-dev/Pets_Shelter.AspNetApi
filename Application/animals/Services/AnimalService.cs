using Application.Account; // for IUserContext
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities;
using Core.Entities.Animal;
using Core.Exceptions;
using Core.Interfaces;
using Ecom.Application.Animals.DTOs;
using Ecom.Application.Animals.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ecom.Application.Animals.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageManagementService _imageService;
        private readonly ILogger<AnimalService> _logger;
        private readonly IUserContext _userContext;
        private readonly UserManager<AppUser> _userManager;

        public AnimalService(IUnitOfWork unitOfWork,
            IImageManagementService imageService,
            ILogger<AnimalService> logger,
            IUserContext userContext,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
            _userContext = userContext;
            _userManager = userManager;
        }

        public async Task<PagedResult<AnimalDTO>> GetAllAsync(AnimalParams animalParams)
        {
            _logger.LogInformation("Executing GetAllAsync with page {PageNumber}, size {PageSize}",
                animalParams.PageNumber, animalParams.PageSize);

            Expression<Func<AdoptionAnimal, bool>>? predicate = null;
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                // Anonymous
                predicate = a => a.AdopterId == null;
            }
            else
            {
                // Find user by ID from the JWT, not by ClaimsPrincipal
                var user = await _userManager.FindByIdAsync(currentUser.Id);
                if (user != null && await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    predicate = a => a.AdopterId == null;
                }
            }

            var result = await _unitOfWork.Animals.GetAllAsync(
                animalParams.PageNumber,
                animalParams.PageSize,
                animalParams.Search,
                animalParams.PetTypeId,
                animalParams.Gender,
                animalParams.AgeFromYears,
                animalParams.AgeToYears,
                animalParams.Sort,
                predicate
            );

            var dto = result.Animals.Select(a => a.ToDto()).ToList();
            return new PagedResult<AnimalDTO>(dto, result.TotalCount, animalParams.PageSize, animalParams.PageNumber);
        }

        public async Task<AnimalDTO?> AddAsync(AddAnimalDTO dto)
        {
            _logger.LogInformation("Executing AddAsync for animal {Name}", dto?.Name);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var animal = dto.ToEntity();
            await _unitOfWork.Animals.AddAsync(animal);
            await _unitOfWork.CompleteAsync();

            if (dto.Photos != null && dto.Photos.Any())
            {
                var imagePaths = await _imageService.AddImageAsync(dto.Photos, dto.Name);
                var photos = imagePaths.Select(p => new AnimalPhoto
                {
                    AnimalId = animal.Id,
                    ImageUrl = p
                }).ToList();

                foreach (var photo in photos)
                    await _unitOfWork.Repository<AnimalPhoto>().AddAsync(photo);

                await _unitOfWork.CompleteAsync();
            }

            return animal.ToDto();
        }

        public async Task<bool> UpdateAsync(UpdateAnimalDTO dto)
        {
            _logger.LogInformation("Executing UpdateAsync for animal Id={Id}", dto?.Id);

            if (dto == null) return false;

            var existing = await _unitOfWork.Animals.GetByidAsync(dto.Id, a => a.Photos, a => a.PetType);
            if (existing == null) return false;

            existing.UpdateEntity(dto);

            // ? Only update photos if new ones are provided
            if (dto.Photos != null && dto.Photos.Any())
            {
                var existingPhotos = existing.Photos?.ToList() ?? new List<AnimalPhoto>();

                // delete old
                foreach (var photo in existingPhotos)
                {
                    _imageService.DeleteImageAsync(photo.ImageUrl);
                    await _unitOfWork.Repository<AnimalPhoto>().DeleteAsync(photo.Id);
                }

                // add new
                var imagePaths = await _imageService.AddImageAsync(dto.Photos, dto.Name);
                var newPhotos = imagePaths.Select(p => new AnimalPhoto
                {
                    AnimalId = dto.Id,
                    ImageUrl = p
                }).ToList();

                foreach (var photo in newPhotos)
                    await _unitOfWork.Repository<AnimalPhoto>().AddAsync(photo);
            }

            await _unitOfWork.Animals.UpdateAsync(dto.Id, existing);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<AnimalDTO?> DeleteAsync(int id)
        {
            _logger.LogInformation("Executing DeleteAsync for animal Id={Id}", id);

            var animal = await _unitOfWork.Animals.GetByidAsync(id, a => a.Photos, a => a.PetType);
            if (animal == null) throw new NotFoundException(nameof(AdoptionAnimal), id.ToString());

            // Delete image files and photo records
            var photos = animal.Photos?.ToList() ?? new List<AnimalPhoto>();
            foreach (var photo in photos)
            {
                try
                {
                    _imageService.DeleteImageAsync(photo.ImageUrl);
                }
                catch
                {
                    // ignore file deletion errors but continue removing DB records
                }

                await _unitOfWork.Repository<AnimalPhoto>().DeleteAsync(photo.Id);
            }

            // Now delete the animal
            await _unitOfWork.Animals.DeleteAsync(animal.Id);
            await _unitOfWork.CompleteAsync();

            return animal.ToDto();
        }

        public async Task<AnimalDTO?> GetAnimalAsync(int id)
        {
            _logger.LogInformation("Executing GetAnimalAsync for animal Id={Id}", id);

            var animal = await _unitOfWork.Animals.GetByidAsync(id, a => a.Photos, a => a.PetType);
            if (animal == null) throw new NotFoundException(nameof(AdoptionAnimal), id.ToString());

            var currentUser = _userContext.GetCurrentUser();
            // hide adopted animals from anonymous users and regular customers
            if ((currentUser == null || currentUser.IsInRole(UserRoles.Customer)) && animal.AdopterId != null)
            {
                throw new NotFoundException(nameof(AdoptionAnimal), id.ToString());
            }

            return animal.ToDto();
        }
    }
}
