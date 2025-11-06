using Application.PetTypes.DTOs;
using Application.PetTypes.Mappings;
using Application.PetTypes.Services;
using Core.Entities.Product;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.PetTypes.Services
{
    public class PetTypeService : IPetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PetTypeService> _logger;

        public PetTypeService(IUnitOfWork unitOfWork, ILogger<PetTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IReadOnlyList<PetTypeDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all pet types...");
            var petTypes = await _unitOfWork.Repository<PetType>().GetAllAsync();

            if (petTypes == null || !petTypes.Any())
                throw new KeyNotFoundException("No pet types found");

            return petTypes.Select(c => c.ToDto()).ToList();
        }

        public async Task<PetTypeDTO> AddAsync(AddPetTypeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Pet type name cannot be empty");

            var petType = dto.ToEntity();

            await _unitOfWork.Repository<PetType>().AddAsync(petType);
            await _unitOfWork.CompleteAsync();

            return petType.ToDto();
        }

        public async Task<PetTypeDTO> UpdateAsync(UpdatePetTypeDTO updateDTO)
        {
            var existing = await _unitOfWork.Repository<PetType>().GetAsync(updateDTO.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Pet type with Id={updateDTO.Id} not found");

            existing.UpdateEntity(updateDTO);

            await _unitOfWork.Repository<PetType>().UpdateAsync(existing.Id, existing);
            await _unitOfWork.CompleteAsync();

            return existing.ToDto();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Repository<PetType>().GetAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Pet type with Id={id} not found");

            await _unitOfWork.Repository<PetType>().DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PetTypeDTO> GetPetTypeAsync(int id)
        {
            var petType = await _unitOfWork.Repository<PetType>().GetAsync(id);
            if (petType == null)
                throw new KeyNotFoundException($"Pet type with Id={id} not found");

            return petType.ToDto();
        }

        public async Task<bool> PetTypeExistsAsync(int id)
        {
            _logger.LogInformation("Checking if pet type with Id {Id} exists...", id);

            var exists = await _unitOfWork.Repository<PetType>().GetAsync(id);
            return exists != null;
        }

    }
}
