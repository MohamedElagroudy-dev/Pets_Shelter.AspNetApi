using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Core.Sharing.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Account.Services
{
    public interface IAccountService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<string?> AddRoleAsync(AddRoleModel model);
        Task<string?> UnassignRoleAsync(UnassignRoleModel model);
        Task<AddressDto> CreateOrUpdateAddress(AddressDto dto);
        Task<UserInfoDto> GetUserInfo();
        Task<UserInfoDto> UpdateProfile(ProfileUpdateDto dto);

        Task<AuthModel?> RefreshToken(string token);
        Task<bool> RevokeToken(string token);
        Task<string> UpdatePictureUrlAsync(UpdatePictureDto dto);
        Task<string> DeletePictureUrlAsync();
        Task<string> GetPictureUrlAsync();
    }
}
