using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Service
{
    public class AdminService : IAdminService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdminService(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<string>> GetAvailableRolesAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();
        }

        //Get all users with search + pagination
        public async Task<(IEnumerable<AppUser> Users, int TotalCount)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? search)
        {
            var query = _userManager.Users
                .AsNoTracking();

            // earch (Username + Email)
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();

                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(searchLower) ||
                    u.Email!.ToLower().Contains(searchLower)
                );
            }

            // ?? Total count (before pagination)
            int totalCount = await query.CountAsync();

            // ?? Pagination
            var users = await query
                .OrderBy(u => u.UserName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }
    }
}
