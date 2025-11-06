using Application.PetTypes.DTOs;

namespace Application.PetTypes.Services
{
    public interface IPetTypeService
    {
        Task<IReadOnlyList<PetTypeDTO>> GetAllAsync();
        Task<PetTypeDTO> GetPetTypeAsync(int id);
        Task<PetTypeDTO> AddAsync(AddPetTypeDTO dto);
        Task<PetTypeDTO> UpdateAsync(UpdatePetTypeDTO updateDTO);
        Task DeleteAsync(int id);
        Task<bool> PetTypeExistsAsync(int id);
    }
}
