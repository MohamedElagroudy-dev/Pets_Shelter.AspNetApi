using Application.Common;
using Application.Common.Pagination;
using Application.donationAnimal.DTOs;
using Application.donationAnimal.Mappings;
using Core.Entities.Animal;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.donationAnimal.Services
{
    public class DonationAnimalService : IDonationAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageManagementService _imageService;
        private readonly ILogger<DonationAnimalService> _logger;

        public DonationAnimalService(
            IUnitOfWork unitOfWork,
            IImageManagementService imageService,
            ILogger<DonationAnimalService> logger)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<PagedResult<DonationAnimalDTO>> GetAllAsync(DonationAnimalParams animalParams)
        {
            _logger.LogInformation("Executing GetAllAsync (Donation) with page {PageNumber}, size {PageSize}",
                animalParams.PageNumber, animalParams.PageSize);

            var result = await _unitOfWork.DonationAnimals.GetAllAsync(
                animalParams.PageNumber,
                animalParams.PageSize,
                animalParams.Search,
                animalParams.PetTypeId,
                animalParams.Gender,
                animalParams.AgeFromYears,
                animalParams.AgeToYears,
                animalParams.Status,
                animalParams.Sort,
                null
            );

            var dto = result.Animals.Select(a => a.ToDto()).ToList();
            return new PagedResult<DonationAnimalDTO>(dto, result.TotalCount, animalParams.PageSize, animalParams.PageNumber);
        }

        public async Task<DonationAnimalDTO?> AddAsync(AddDonationAnimalDTO dto)
        {
            _logger.LogInformation("Executing AddAsync for donation animal {Name}", dto?.Name);

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var animal = dto.ToEntity();
            await _unitOfWork.DonationAnimals.AddAsync(animal);
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

        public async Task<bool> UpdateAsync(UpdateDonationAnimalDTO dto)
        {
            _logger.LogInformation("Executing UpdateAsync for donation animal Id={Id}", dto?.Id);

            if (dto == null) return false;

            var existing = await _unitOfWork.DonationAnimals.GetByidAsync(dto.Id, a => a.Photos, a => a.PetType);
            if (existing == null) return false;

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

            await _unitOfWork.DonationAnimals.UpdateAsync(dto.Id, existing);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<DonationAnimalDTO?> DeleteAsync(int id)
        {
            _logger.LogInformation("Executing DeleteAsync for donation animal Id={Id}", id);

            var animal = await _unitOfWork.DonationAnimals.GetByidAsync(id, a => a.Photos, a => a.PetType);
            if (animal == null) throw new NotFoundException(nameof(DonationAnimal), id.ToString());

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

            await _unitOfWork.DonationAnimals.DeleteAsync(animal.Id);
            await _unitOfWork.CompleteAsync();

            return animal.ToDto();
        }

        public async Task<DonationAnimalDTO?> GetAnimalAsync(int id)
        {
            _logger.LogInformation("Executing GetAnimalAsync (Donation) for Id={Id}", id);

            var animal = await _unitOfWork.DonationAnimals.GetByidAsync(id, a => a.Photos, a => a.PetType, a => a.Donations);
            if (animal == null) throw new NotFoundException(nameof(DonationAnimal), id.ToString());

            return animal.ToDto();
        }
    }
}