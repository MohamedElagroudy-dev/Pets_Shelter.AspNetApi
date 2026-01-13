using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class AdminService : IAdminService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> GetAvailableRolesAsync()
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();
            
            return roles!;
        }
    }
}
