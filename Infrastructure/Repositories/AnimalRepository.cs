using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.Animal;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AnimalRepository : GenericRepository<AdoptionAnimal>, IAnimalRepository
    {
        private readonly ApplicationDbContext _context;

        public AnimalRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<AdoptionAnimal> Animals, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? petTypeId,
            Gender? gender,
            double? ageFromYears,
            double? ageToYears,
            AnimalSort? sort,
            Expression<Func<AdoptionAnimal, bool>>? predicate = null)
        {
            var query = _context.Set<AdoptionAnimal>()
                .Include(a => a.Photos)
                .Include(a => a.PetType)
                .AsNoTracking();

            // Apply custom predicate if provided
            if (predicate != null)
                query = query.Where(predicate);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchWords = search.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(a => searchWords.All(word =>
                    a.Name.ToLower().Contains(word.ToLower()) ||
                    a.Description.ToLower().Contains(word.ToLower())
                ));
            }

            if (petTypeId.HasValue)
                query = query.Where(a => a.PetTypeId == petTypeId.Value);

            if (gender.HasValue)
                query = query.Where(a => a.Gender == gender.Value);

            if (ageFromYears.HasValue)
                query = query.Where(a => a.AgeYears >= ageFromYears.Value);

            if (ageToYears.HasValue)
                query = query.Where(a => a.AgeYears <= ageToYears.Value);

            var totalCount = await query.CountAsync();

            query = sort switch
            {
                AnimalSort.NameAsc => query.OrderBy(a => a.Name),
                AnimalSort.NameDesc => query.OrderByDescending(a => a.Name),
                AnimalSort.AgeAsc => query.OrderBy(a => a.AgeYears),
                AnimalSort.AgeDesc => query.OrderByDescending(a => a.AgeYears),
                AnimalSort.CreatedAtAsc => query.OrderBy(a => a.CreatedAt),
                AnimalSort.CreatedAtDesc => query.OrderByDescending(a => a.CreatedAt),
                _ => query.OrderByDescending(a => a.Id)
            };

            var animals = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (animals, totalCount);
        }
    }
}
