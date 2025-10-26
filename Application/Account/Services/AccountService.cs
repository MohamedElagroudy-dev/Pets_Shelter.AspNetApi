using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Application.Account.Mappings;
using Core.Entities;
using Core.Interfaces;
using Core.Sharing.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Claims;

namespace Application.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserContext _userContext;


        
        public AccountService(IAuthService authService,
            ILogger<AccountService> logger,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserContext userContext)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = userContext;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("User registration for email: {Email}", model.Email);
            return await _authService.RegisterAsync(model);
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("Token request for email: {Email}", model.Email);
            return await _authService.GetTokenAsync(model);
        }

        public async Task<string?> AddRoleAsync(AddRoleModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email))
                throw new ArgumentException("Email is required");

            _logger.LogInformation("Adding role '{Role}' to user: {UserId}", model.Role, model.Email);
            return await _authService.AddRoleAsync(model);
        }
        public async Task<string?> UnassignRoleAsync(UnassignRoleModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email))
                throw new ArgumentException("Email is required");

            _logger.LogInformation("Unassign role '{Role}' to user: {UserId}", model.Role, model.Email);
            return await _authService.UnassignUserRole(model);
        }
        public async Task<AuthModel?> RefreshToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required");

            _logger.LogInformation("Refreshing token for user: {UserId}", _userContext.GetCurrentUser()?.Email);
            return await _authService.RefreshTokenAsync(token);
        }
        public async Task<bool> RevokeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required");

            _logger.LogInformation("Revoke Token for user: {UserId}", _userContext.GetCurrentUser()?.Email);
            return await _authService.RevokeTokenAsync(token);
        }




        public async Task<AddressDto> CreateOrUpdateAddress(AddressDto dto)
        {
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            _logger.LogInformation("create-or-update Address");

            var entity = dto.ToEntity();

            var updatedEntity = await _authService.CreateOrUpdateAddressAsync(currentUser.Email!, entity);


            var UpdatedEntityToReturn = updatedEntity.ToDto();

            if (UpdatedEntityToReturn == null)
                throw new ArgumentNullException(nameof(UpdatedEntityToReturn));

            return UpdatedEntityToReturn;
        }
        public async Task<UserInfoDto> GetUserInfo()
        {
            _logger.LogInformation("Getting user info");
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var (user, roles) = await _authService.GetUserByEmailWithAddress(currentUser.Email!);

            if (user == null)
                throw new InvalidOperationException("User not found");

            var userInfo = user.ToDto() ?? new UserInfoDto();

            userInfo.Roles = roles?.ToList() ?? new List<string>();

            return userInfo;
        }

    }
}