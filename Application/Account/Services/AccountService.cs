using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Application.Account.Mappings;
using Core.Entities;
using Core.Interfaces;
using Core.Sharing.Identity;
using Microsoft.AspNetCore.Http;
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
        private readonly IImageManagementService _imageService;

        private const string DefaultImagePath = "/Images/Defult/DefultUserPic.jpeg";



        public AccountService(IAuthService authService,
            ILogger<AccountService> logger,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserContext userContext,
            IImageManagementService imageService)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = userContext;
            _imageService = imageService;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("User registration for email: {Email}", model.Email);
            var authModel = await _authService.RegisterAsync(model);
            if (string.IsNullOrWhiteSpace(authModel.PictureUrl) && authModel.IsAuthenticated)
            {
                authModel.PictureUrl = DefaultImagePath;
            }
            return authModel;
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("Token request for email: {Email}", model.Email);
            var token = await _authService.GetTokenAsync(model);
            if (string.IsNullOrWhiteSpace(token.PictureUrl) && token.IsAuthenticated)
            {
                token.PictureUrl = DefaultImagePath;
            }
            return token;
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

        public async Task<UserInfoDto> UpdateProfile(ProfileUpdateDto dto)
        {
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var (user, roles) = await _authService.GetUserByEmailWithAddress(currentUser.Email!);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Update simple properties
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;

            // Update address
            if (dto.Address != null)
            {
                if (user.Address == null)
                {
                    user.Address = dto.Address.ToEntity();
                }
                else
                {
                    user.Address.UpdateFromDto(dto.Address);
                }
            }

            var updated = await _authService.UpdateUserAsync(user);

            var userInfo = updated.ToDto() ?? new UserInfoDto();
            userInfo.Roles = roles?.ToList() ?? new List<string>();

            return userInfo;
        }

        public async Task<string> UpdatePictureUrlAsync(UpdatePictureDto dto)
        {
            _logger.LogInformation("Updating picture for user: {Email}", _userContext.GetCurrentUser()?.Email);

            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            if (dto?.Picture == null)
                throw new ArgumentNullException(nameof(dto.Picture), "Picture file is required");

            if (dto.Picture.Length == 0)
                throw new ArgumentException("Picture file is empty");

            // Delete old picture if exists before uploading new one
            var user = await _userManager.FindByEmailAsync(currentUser.Email);
            if (user != null && !string.IsNullOrEmpty(user.PictureUrl) && user.PictureUrl != string.Empty)
            {
                _imageService.DeleteImageAsync(user.PictureUrl);
            }

        

            var imagePaths = await _imageService.AddSingleImageAsync(dto.Picture,"Users");

            if (string.IsNullOrEmpty(imagePaths))
                throw new Exception("Failed to upload picture");

            // Update picture URL in database
            var updatedUrl = await _authService.UpdatePictureUrlAsync(currentUser.Email!, imagePaths);

            _logger.LogInformation("Picture updated successfully for user: {Email}", currentUser.Email);

            return updatedUrl;
        }


        public async Task<string> DeletePictureUrlAsync()
        {
            _logger.LogInformation("Deleting picture for user: {Email}", _userContext.GetCurrentUser()?.Email);

            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            await _authService.DeletePictureUrlAsync(currentUser.Email!);

            _logger.LogInformation("Picture deleted successfully for user: {Email}", currentUser.Email);

            return string.Empty;
        }

        public async Task<string> GetPictureUrlAsync()
        {
            _logger.LogInformation("Getting picture URL for user: {Email}", _userContext.GetCurrentUser()?.Email);

            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var pictureUrl = await _authService.GetPictureUrlAsync(currentUser.Email!);

            return string.IsNullOrEmpty(pictureUrl) ? DefaultImagePath : pictureUrl;
        }

    }
}