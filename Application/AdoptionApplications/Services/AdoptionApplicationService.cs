using Application.Account;
using Application.AdoptionApplications.DTOs;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.AdoptionApp;
using Core.Entities.Animal;
using Core.Interfaces;
using Ecom.Application.AdoptionApplications.DTOs;
using Ecom.Application.AdoptionApplications.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.Application.AdoptionApplications.Services
{
    public class AdoptionApplicationService : IAdoptionApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IAuthService _authService;

        public AdoptionApplicationService(IUnitOfWork unitOfWork, IUserContext userContext, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _authService = authService;
        }

        public async Task<int> CreateAsync(CreateAdoptionApplicationDto dto, string userId)
        {
            var animal = await _unitOfWork.Repository<AdoptionAnimal>().GetAsync(dto.AnimalId);
            if (animal == null) throw new ArgumentException("Animal not found");

            var existing = await _unitOfWork.Repository<AdoptionApplication>()
                .GetByAsync(a => a.AnimalId == dto.AnimalId && a.ApplicantId == userId);
            if (existing != null)
                throw new ArgumentException("You have already submitted an application for this animal.");

            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var (user, roles) = await _authService.GetUserByEmailWithAddress(currentUser.Email!);

            if (user == null)
                throw new InvalidOperationException("User not found");

            var entity = dto.ToEntity(user);

            await _unitOfWork.Repository<AdoptionApplication>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<PagedResult<AdoptionApplicationDto>> GetMyApplicationsAsync(string userId, AdoptionApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, userId, @params.Status, @params.Sort);

            var dtos = items.Select(a => a.ToDto()).ToList();
            return new PagedResult<AdoptionApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
        }

        public async Task<AdoptionApplicationDetailsDto?> GetMyApplicationByIdAsync(string userId, int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id && a.ApplicantId == userId, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }

        // ADMIN
        public async Task<AdoptionApplicationStatsResult> GetAllAsync(AdoptionApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, null, @params.Status, @params.Sort);
            var dtos = items.Select(a => a.ToDto()).ToList();

            // compute counts
            var ApprovedAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Approved, @params.Sort);
            var ApprovedCount = ApprovedAll.TotalCount; // total count of all (active)

            var RejectedAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Rejected, @params.Sort);
            var RejectedCount = RejectedAll.TotalCount; // total count of all (active)

            var pendingAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Pending, @params.Sort);
            var pendingCount = pendingAll.TotalCount;

            var pagedResult = new PagedResult<AdoptionApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
            return new AdoptionApplicationStatsResult
            {
                PagedResult = pagedResult,
                ApprovedRequestsCount = ApprovedCount,
                RejectedRequestsCount = RejectedCount,
                PendingRequestsCount = pendingCount,
                SuccessRate = (ApprovedCount + RejectedCount) > 0
                    ? Math.Round((double)ApprovedCount / (ApprovedCount + RejectedCount) * 100, 2)
                    : 0
            };
        }

        public async Task<AdoptionApplicationDetailsDto?> GetByIdAsync(int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }
    }

   
}
