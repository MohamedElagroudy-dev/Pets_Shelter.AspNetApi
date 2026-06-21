using Core.Constants;
using Core.Entities.Animal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDonationAnimalRepository : IGenericRepository<DonationAnimal>
    {
        Task<(IEnumerable<DonationAnimal> Animals, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? petTypeId,
            Gender? gender,
            double? ageFromYears,
            double? ageToYears,
            DonationStatus? status,
            AnimalSort? sort,
            Expression<Func<DonationAnimal, bool>>? predicate = null);
    }
}
