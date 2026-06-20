using Application.Admin.DTO;
using Application.Admin.Mappings;
using Application.Common;
using Application.Common.Pagination;
using Core.Entities;
using Core.Interfaces;
using Ecom.Application.Products.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecom.Application.Animals.Mappings;
using Ecom.Application.FosterAnimals.Mappings;

namespace Application.Admin.Services
{
    public class AdminAppService : IAdminAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminAppService> _logger;

        public AdminAppService(IUnitOfWork unitOfWork, ILogger<AdminAppService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetAvailableRolesAsync()
        {
            _logger?.LogInformation("GetAvailableRolesAsync called");

            if (_unitOfWork?.AdminService == null)
            {
                _logger?.LogWarning("AdminService is not available on UnitOfWork");
                return Enumerable.Empty<string>();
            }

            var roles = await _unitOfWork.AdminService.GetAvailableRolesAsync() ?? Enumerable.Empty<string>();

            return roles;
        }

        public async Task<PagedResult<UserDto>> GetAllUsersAsync(UserParams paginationParams)
        {
            _logger?.LogInformation($"GetAllUsersAsync called with pageNumber=" +
                $"{paginationParams.PageNumber}, pageSize={paginationParams.PageSize}, search={paginationParams.Search}, role={paginationParams.Role}");

            if (_unitOfWork?.AdminService == null)
            {
                _logger?.LogWarning("AdminService is not available on UnitOfWork");
                return new PagedResult<UserDto>(new List<UserDto>(), 0, paginationParams.PageSize, paginationParams.PageNumber);
            }

            var usersTuple = await _unitOfWork.AdminService.GetAllUsersAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                paginationParams.Search,
                paginationParams.Role);

            var result = usersTuple.Users;
            var count = usersTuple.TotalCount;

            var users = result.Select(c => c.ToDto()).ToList();

            // fetch roles for each user sequentially
            foreach (var u in users)
            {
                var role = await _unitOfWork.AdminService.GetUserPrimaryRoleAsync(u.Id);
                u.Role = role ?? string.Empty;
            }

            return new PagedResult<UserDto>(users, count, paginationParams.PageSize, paginationParams.PageNumber);
        }

        public async Task<UserDetailsDto?> GetUserDetailsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            // Fetch user via admin service
            var user = await _unitOfWork.AdminService.GetUserByIdAsync(userId);
            if (user == null) return null;

            var userDto = user.ToDto();

            // Fetch adopted animals where AdopterId == userId
            var (adoptedAnimals, adoptedCount) = await _unitOfWork.Animals.GetAllAsync(1, int.MaxValue, null, null, null, null, null, null, a => a.AdopterId == userId);
            var adoptedDtos = adoptedAnimals.Select(a => a.ToDto()).ToList();

            // Fetch foster animals where FostererId == userId
            var (fosteredAnimals, fosteredCount) = await _unitOfWork.FosterAnimals.GetAllAsync(1, int.MaxValue, null, null, null, null, null, null, a => a.FostererId == userId);
            var fosteredDtos = fosteredAnimals.Select(a => a.ToDto()).ToList();

            var result = new UserDetailsDto
            {
                User = userDto,
                AdoptedAnimals = adoptedDtos,
                FosteredAnimals = fosteredDtos
            };

            return result;
        }
    }
}
