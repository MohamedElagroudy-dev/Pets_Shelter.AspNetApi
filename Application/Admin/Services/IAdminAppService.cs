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
    }
}
