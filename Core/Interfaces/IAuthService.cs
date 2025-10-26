using Core.Entities;
using Core.Sharing.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<string> UnassignUserRole(UnassignRoleModel model);
        Task<Address> CreateOrUpdateAddressAsync(string userEmail, Address newAddress);
        Task<(AppUser, IEnumerable<string>)> GetUserByEmailWithAddress(string userEmail);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);


    }
}
