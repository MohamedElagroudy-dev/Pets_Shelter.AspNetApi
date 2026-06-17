using Application.Admin.DTO;
using Application.Common;
using Application.Common.Pagination;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Services
{
    public interface IAdminAppService
    {
        Task<IEnumerable<string>> GetAvailableRolesAsync();
        Task<PagedResult<UserDto>> GetAllUsersAsync(UserParams paginationParams);
    }
}
