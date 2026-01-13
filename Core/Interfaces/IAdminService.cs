using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<string>> GetAvailableRolesAsync();
    }
}
