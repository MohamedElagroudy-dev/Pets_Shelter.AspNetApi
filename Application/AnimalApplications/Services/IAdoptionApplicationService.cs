using Ecom.Application.AnimalApplications.DTOs;
using Application.Common;
using Core.Constants;
using Application.Common.Pagination;

namespace Ecom.Application.AnimalApplications.Services
{
    public interface IAnimalApplicationService
    {
        Task<int> CreateAdoptionAsync(CreateAnimalApplicationDto dto, string userId);
        Task<int> CreateFosterAsync(CreateAnimalApplicationDto dto, string userId);
        Task<PagedResult<AnimalApplicationDto>> GetMyApplicationsAsync(string userId, AnimalApplicationParams @params);
        Task<AnimalApplicationDetailsDto?> GetMyApplicationByIdAsync(string userId, int id);

        // Admin
        Task<AnimalApplicationStatsResult> GetAllAsync(AnimalApplicationParams @params);
        Task<AnimalApplicationDetailsDto?> GetByIdAsync(int id);
    }
}
