using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<string>> GetAvailableRolesAsync();
        Task<(IEnumerable<AppUser> Users, int TotalCount)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? role);

        Task<string?> GetUserPrimaryRoleAsync(string userId);
        Task<AppUser?> GetUserByIdAsync(string userId); // Get user by id
    }
}
