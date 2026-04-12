using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        //Get all users with search + pagination + optional role filter
        public async Task<(IEnumerable<AppUser> Users, int TotalCount)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? role)
        {
            // If a role is provided, get users in that role (in-memory list) and apply search/pagination there.
            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var query = usersInRole.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(u =>
                        (u.UserName ?? string.Empty).ToLower().Contains(searchLower) ||
                        (u.Email ?? string.Empty).ToLower().Contains(searchLower)
                    );
                }

                var totalCount = query.Count();

                var users = query
                    .OrderBy(u => u.UserName)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToList();

                return (users, totalCount);
            }

            // No role filter – use IQueryable from UserManager for server-side filtering/pagination.
            var baseQuery = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                baseQuery = baseQuery.Where(u =>
                    (u.UserName ?? string.Empty).ToLower().Contains(searchLower) ||
                    (u.Email ?? string.Empty).ToLower().Contains(searchLower)
                );
            }

            int total = await baseQuery.CountAsync();

            var pagedUsers = await baseQuery
                .OrderBy(u => u.UserName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (pagedUsers, total);
        }

        public async Task<string?> GetUserPrimaryRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
    }
}
