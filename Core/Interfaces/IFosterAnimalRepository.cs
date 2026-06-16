using Core.Entities.Animal;
using Core.Constants;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFosterAnimalRepository : IGenericRepository<FosterAnimal>
    {
        Task<(IEnumerable<FosterAnimal> Animals, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? petTypeId,
            Gender? gender,
            double? ageFromYears,
            double? ageToYears,
            AnimalSort? sort,
            Expression<Func<FosterAnimal, bool>>? predicate = null);
    }
}
