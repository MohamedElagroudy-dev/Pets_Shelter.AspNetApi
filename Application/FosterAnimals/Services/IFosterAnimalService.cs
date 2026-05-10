using Ecom.Application.FosterAnimals.DTOs;
using Application.Common.Pagination;
using System.Threading.Tasks;
using Application.Common;

namespace Ecom.Application.FosterAnimals.Services
{
    public interface IFosterAnimalService
    {
        Task<PagedResult<FosterAnimalDTO>> GetAllAsync(AnimalParams animalParams);
        Task<FosterAnimalDTO?> AddAsync(AddFosterAnimalDTO dto);
        Task<bool> UpdateAsync(UpdateFosterAnimalDTO dto);
        Task<FosterAnimalDTO?> DeleteAsync(int id);
        Task<FosterAnimalDTO?> GetFosterAnimalAsync(int id);
    }
}
