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
                return new PagedResult<UserDto>(new List<UserDto>(), 0, paginationParams.PageNumber, paginationParams.PageSize);
            }

            var usersTuple = await _unitOfWork.AdminService.GetAllUsersAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                paginationParams.Search,
                paginationParams.Role);

            var result = usersTuple.Users;
            var count = usersTuple.TotalCount;

            var users = result.Select(c => c.ToDto()).ToList();
            return new PagedResult<UserDto>(users, count, paginationParams.PageSize, paginationParams.PageNumber);
        }
    }
}
