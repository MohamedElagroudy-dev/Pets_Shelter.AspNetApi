using Core.Constants;
using Core.Entities.AdoptionApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAdoptionApplicationRepository : IGenericRepository<AdoptionApplication>
    {
        Task<(IEnumerable<AdoptionApplication> Items, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? applicantId,
            ApplicationStatus? status,
            AdoptionApplicationSort sort,
            ApplicationType? applicationType);

        Task<AdoptionApplication?> RejectAsync(int id, string adminNotes);
        Task<AdoptionApplication?> AcceptAsync(int id, string adminNotes);
    }
}
