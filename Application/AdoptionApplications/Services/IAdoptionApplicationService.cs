using Ecom.Application.AdoptionApplications.DTOs;
using Application.Common;
using Core.Constants;
using Application.Common.Pagination;
using Application.AdoptionApplications.DTOs;

namespace Ecom.Application.AdoptionApplications.Services
{
    public interface IAdoptionApplicationService
    {
        Task<int> CreateAsync(CreateAdoptionApplicationDto dto, string userId);
        Task<PagedResult<AdoptionApplicationDto>> GetMyApplicationsAsync(string userId, AdoptionApplicationParams @params);
        Task<AdoptionApplicationDetailsDto?> GetMyApplicationByIdAsync(string userId, int id);

        // Admin
        Task<AdoptionApplicationStatsResult> GetAllAsync(AdoptionApplicationParams @params);
        Task<AdoptionApplicationDetailsDto?> GetByIdAsync(int id);
    }
}
