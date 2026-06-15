using Application.Account;
using Ecom.Application.AnimalApplications.DTOs;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.AdoptionApp;
using Core.Entities.Animal;
using Core.Interfaces;
using Ecom.Application.AnimalApplications.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.Application.AnimalApplications.Services
{
    public class AnimalApplicationService : IAnimalApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;

        public AnimalApplicationService(
            IUnitOfWork unitOfWork, 
            IUserContext userContext, 
            IAuthService authService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _authService = authService;
            _notificationService = notificationService;
        }

        public async Task<int> CreateAdoptionAsync(CreateAnimalApplicationDto dto, string userId)
        {
            var animal = await _unitOfWork.Repository<BaseAnimal>().GetAsync(dto.AnimalId);
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
        public async Task<int> CreateFosterAsync(CreateAnimalApplicationDto dto, string userId)
        {
            var animal = await _unitOfWork.Repository<FosterAnimal>().GetAsync(dto.AnimalId);
            if (animal == null)
                throw new ArgumentException("Animal not found");

            // Ensure animal is available for fostering
            if (animal.Status != FosterStatus.Available || animal.IsFostered || animal.HasLeftShelter)
                throw new ArgumentException("This animal is not available for fostering");

            var existing = await _unitOfWork.Repository<AdoptionApplication>()
                .GetByAsync(a => a.AnimalId == dto.AnimalId
                              && a.ApplicantId == userId);
            if (existing != null)
                throw new ArgumentException("You have already submitted a foster application for this animal.");

            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var (user, _) = await _authService.GetUserByEmailWithAddress(currentUser.Email!);
            if (user == null)
                throw new InvalidOperationException("User not found");

            dto.ApplicationType = ApplicationType.Foster; 
            var entity = dto.ToEntity(user);
            await _unitOfWork.Repository<AdoptionApplication>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<PagedResult<AnimalApplicationDto>> GetMyApplicationsAsync(string userId, AnimalApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, userId, @params.Status, @params.Sort, @params.ApplicationType);

            var dtos = items.Select(a => a.ToDto()).ToList();
            return new PagedResult<AnimalApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
        }

        public async Task<AnimalApplicationDetailsDto?> GetMyApplicationByIdAsync(string userId, int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id && a.ApplicantId == userId, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }

        // ADMIN
        public async Task<AnimalApplicationStatsResult> GetAllAsync(AnimalApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, null, @params.Status, @params.Sort,@params.ApplicationType);
            var dtos = items.Select(a => a.ToDto()).ToList();

            // compute counts
            var ApprovedAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Approved, @params.Sort,@params.ApplicationType);
            var ApprovedCount = ApprovedAll.TotalCount; // total count of all (active)

            var RejectedAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Rejected, @params.Sort,@params.ApplicationType);
            var RejectedCount = RejectedAll.TotalCount; // total count of all (active)

            var pendingAll = await _unitOfWork.AdoptionApplications.GetAllAsync(1, int.MaxValue, @params.Search, null, ApplicationStatus.Pending, @params.Sort,@params.ApplicationType);
            var pendingCount = pendingAll.TotalCount;

            var pagedResult = new PagedResult<AnimalApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
            return new AnimalApplicationStatsResult
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

        public async Task<AnimalApplicationDetailsDto?> GetByIdAsync(int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }

        public async Task<AnimalApplicationDetailsDto?> RejectApplicationAsync(int id, RejectApplicationDto dto)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>()
                .GetByAsync(a => a.Id == id, a => a.Animal, a => a.Animal.Photos, a => a.Applicant);
            
            if (app == null)
                throw new KeyNotFoundException($"Application with ID {id} not found");

            // Update application status and admin notes
            app.Status = ApplicationStatus.Rejected;
            app.AdminNotes = dto.AdminNotes;
            app.ReviewedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<AdoptionApplication>().UpdateAsync(id, app);
            await _unitOfWork.CompleteAsync();

            // Send notification to the applicant
            await _notificationService.NotifyApplicationRejectedAsync(app);

            return app.ToDetailsDto();
        }
    }

   
}
