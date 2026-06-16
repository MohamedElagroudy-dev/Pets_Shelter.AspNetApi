using Application.Account;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities;
using Core.Entities.Animal;
using Core.Interfaces;
using Ecom.Application.FosterAnimals.DTOs;
using Ecom.Application.FosterAnimals.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ecom.Application.FosterAnimals.Services
{
    public class FosterAnimalService : IFosterAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageManagementService _imageService;
        private readonly ILogger<FosterAnimalService> _logger;
        private readonly IUserContext _userContext;
        private readonly UserManager<AppUser> _userManager;

        public FosterAnimalService(IUnitOfWork unitOfWork,
            IImageManagementService imageService,
            ILogger<FosterAnimalService> logger,
            IUserContext userContext,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
            _userContext = userContext;
            _userManager = userManager;
        }

        public async Task<PagedResult<FosterAnimalDTO>> GetAllAsync(AnimalParams animalParams)
        {
            _logger.LogInformation("Executing GetAllAsync for FosterAnimal with page {PageNumber}, size {PageSize}",
                animalParams.PageNumber, animalParams.PageSize);

            Expression<Func<FosterAnimal, bool>>? predicate = null;
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                // Anonymous
                predicate = a => a.FostererId == null;
            }
            else
            {
                // Find user by ID from the JWT, not by ClaimsPrincipal
                var user = await _userManager.FindByIdAsync(currentUser.Id);
                if (user != null && await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    predicate = a => a.FostererId == null;
                }
            }
            var result = await _unitOfWork.FosterAnimals.GetAllAsync(
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

            var animals = result.Animals;
            var totalCount = result.TotalCount;

            var dto = animals.Select(a => a.ToDto()).ToList();

            return new PagedResult<FosterAnimalDTO>(dto, totalCount, animalParams.PageSize, animalParams.PageNumber);
        }

        public async Task<FosterAnimalDTO?> AddAsync(AddFosterAnimalDTO dto)
        {
            _logger.LogInformation("Executing AddAsync for foster animal {Name}", dto?.Name);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var animal = dto.ToEntity();
            await _unitOfWork.FosterAnimals.AddAsync(animal);
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

        public async Task<bool> UpdateAsync(UpdateFosterAnimalDTO dto)
        {
            _logger.LogInformation("Executing UpdateAsync for foster animal Id={Id}", dto?.Id);

            if (dto == null) return false;

            var existing = await _unitOfWork.FosterAnimals.GetByidAsync(dto.Id, a => a.Photos, a => a.PetType);
            if (existing == null) return false;

            UpdateFosterStatusIfExpired(existing);

            existing.UpdateEntity(dto);

            if (dto.Photos != null && dto.Photos.Any())
            {
                var existingPhotos = existing.Photos?.ToList() ?? new List<AnimalPhoto>();
                foreach (var photo in existingPhotos)
                {
                    _imageService.DeleteImageAsync(photo.ImageUrl);
                    await _unitOfWork.Repository<AnimalPhoto>().DeleteAsync(photo.Id);
                }
                var imagePaths = await _imageService.AddImageAsync(dto.Photos, dto.Name);
                var newPhotos = imagePaths.Select(p => new AnimalPhoto
                {
                    AnimalId = dto.Id,
                    ImageUrl = p
                }).ToList();
                foreach (var photo in newPhotos)
                    await _unitOfWork.Repository<AnimalPhoto>().AddAsync(photo);
            }

            await _unitOfWork.FosterAnimals.UpdateAsync(dto.Id, existing);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<FosterAnimalDTO?> DeleteAsync(int id)
        {
            _logger.LogInformation("Executing DeleteAsync for foster animal Id={Id}", id);

            var animal = await _unitOfWork.FosterAnimals.GetByidAsync(id, a => a.Photos, a => a.PetType);
            if (animal == null) return null;

            var photos = animal.Photos?.ToList() ?? new List<AnimalPhoto>();
            foreach (var photo in photos)
            {
                try { _imageService.DeleteImageAsync(photo.ImageUrl); } catch { }
                await _unitOfWork.Repository<AnimalPhoto>().DeleteAsync(photo.Id);
            }

            await _unitOfWork.FosterAnimals.DeleteAsync(animal.Id);
            await _unitOfWork.CompleteAsync();

            return animal.ToDto();
        }

        public bool UpdateFosterStatusIfExpired(FosterAnimal animal)
        {
            if (!animal.FosterEndDate.HasValue)
                return false;
            if (animal.FosterEndDate > DateTime.UtcNow)
                return false;
            if (animal.Status != FosterStatus.InFoster)
                return false;
            animal.Status = animal.HasLeftShelter ? FosterStatus.Completed : FosterStatus.Cancelled;
            return true;
        }

        public async Task<FosterAnimalDTO?> GetFosterAnimalAsync(int id)
        {
            _logger.LogInformation("Executing GetFosterAnimalAsync for foster animal Id={Id}", id);
            var animal = await _unitOfWork.FosterAnimals.GetByidAsync(id, a => a.Photos, a => a.PetType);
            if (animal == null) return null;
            UpdateFosterStatusIfExpired(animal);
            return animal.ToDto();
        }
    }
}
