using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.Animal;
using Ecom.Application.FosterAnimals.DTOs;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ecom.Application.FosterAnimals.Services
{
    public interface IFosterAnimalService
    {
        Task<PagedResult<FosterAnimalDTO>> GetAllAsync(AnimalParams animalParams);
        Task<FosterAnimalDTO?> AddAsync(AddFosterAnimalDTO dto);
        Task<bool> UpdateAsync(UpdateFosterAnimalDTO dto);
        Task<FosterAnimalDTO?> DeleteAsync(int id);
        Task<FosterAnimalDTO?> GetFosterAnimalAsync(int id);
        Task<PagedResult<FosterAnimalDTO>> GetAllMyAsync(AnimalParams animalParams);
        Task<PagedResult<FosterAnimalDTO>> GetAllFosteredAsync(AnimalParams animalParams);
        Task<PagedResult<FosterAnimalDTO>> GetAllFosterEndedAsync(AnimalParams animalParams, FosterStatus? status);
    }
}
