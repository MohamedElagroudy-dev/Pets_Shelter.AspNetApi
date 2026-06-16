using Core.Entities.Animal;
using Core.Constants;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAnimalRepository : IGenericRepository<AdoptionAnimal>
    {
        Task<(IEnumerable<AdoptionAnimal> Animals, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? petTypeId,
            Gender? gender,
            double? ageFromYears,
            double? ageToYears,
            AnimalSort? sort,
            Expression<Func<AdoptionAnimal, bool>>? predicate = null);
    }
}
