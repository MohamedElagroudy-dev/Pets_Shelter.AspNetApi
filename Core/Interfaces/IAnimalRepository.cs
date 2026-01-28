using Core.Entities.Animal;
using Core.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        Task<(IEnumerable<Animal> Animals, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? petTypeId,
            Gender? gender,
            double? ageFromYears,
            double? ageToYears,
            AnimalSort? sort);
    }
}
