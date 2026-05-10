using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.AdoptionApp;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AdoptionApplicationRepository : GenericRepository<AdoptionApplication>, IAdoptionApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public AdoptionApplicationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<AdoptionApplication> Items, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? applicantId,
            ApplicationStatus? status,
            AdoptionApplicationSort sort,
            ApplicationType? applicationType
            )
        {
            var query = _context.Set<AdoptionApplication>()
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Photos)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(a => a.ApplicantInfo.FirstName.ToLower().Contains(s)
                                         || a.ApplicantInfo.LastName.ToLower().Contains(s)
                                         || a.ApplicantInfo.Email.ToLower().Contains(s)
                                         || a.Animal.Name.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(applicantId))
                query = query.Where(a => a.ApplicantId == applicantId);

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            if (applicationType.HasValue)
                query = query.Where(a => a.ApplicationType == applicationType.Value);

            var total = await query.CountAsync();

            query = sort switch
            {
                AdoptionApplicationSort.SubmittedAtAsc => query.OrderBy(a => a.SubmittedAt),
                AdoptionApplicationSort.SubmittedAtDesc => query.OrderByDescending(a => a.SubmittedAt),
                _ => query.OrderByDescending(a => a.Id)
            };

            var items = await query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}
