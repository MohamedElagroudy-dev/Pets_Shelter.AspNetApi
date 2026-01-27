using Application.Common.Pagination;
using Application.Common;
using Ecom.Application.Animals.DTOs;

namespace Ecom.Application.Animals.Services
{
    public interface IAnimalService
    {
        Task<PagedResult<AnimalDTO>> GetAllAsync(AnimalParams animalParams);
        Task<AnimalDTO?> AddAsync(AddAnimalDTO dto);
        Task<bool> UpdateAsync(UpdateAnimalDTO dto);
        Task<AnimalDTO?> DeleteAsync(int id);
        Task<AnimalDTO?> GetAnimalAsync(int id);
    }
}
