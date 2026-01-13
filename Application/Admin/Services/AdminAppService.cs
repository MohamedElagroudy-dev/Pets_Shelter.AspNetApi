using Core.Interfaces;
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
    }
}
