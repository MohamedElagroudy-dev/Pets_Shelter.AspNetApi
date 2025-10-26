using Application.Account;
using Azure.Core;
using Core.Entities;
using Core.Entities.Product;
using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces;
using Core.Sharing;
using Core.Sharing.Identity;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IUserContext _userContext;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IUserContext userContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _userContext = userContext;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Customer);

            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { UserRoles.Customer },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Roles = rolesList.ToList();

            if (user.RefreshTokens != null && user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                if (activeRefreshToken != null)
                {
                    authModel.RefreshToken = activeRefreshToken.Token;
                    authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
                }
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        public async Task<string> UnassignUserRole(UnassignRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
                ?? throw new NotFoundException(nameof(AppUser), model.Email);

            var role = await _roleManager.FindByNameAsync(model.Role)
                ?? throw new NotFoundException(nameof(IdentityRole), model.Role);

            var currentUser = _userContext.GetCurrentUser();

            // Prevent self-admin unassignment
            if (model.Role == UserRoles.Admin && currentUser?.Email == model.Email)
            {
                throw new InvalidOperationException("You cannot remove your own Admin role.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim("roles", r)).ToList();

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("uid", user.Id)
                };

            if (!string.IsNullOrEmpty(user.FirstName))
                claims.Add(new Claim("first_name", user.FirstName));

            if (!string.IsNullOrEmpty(user.LastName))
                claims.Add(new Claim("last_name", user.LastName));

            // Now merge all
            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: creds
            );
        }

        public async Task<Address> CreateOrUpdateAddressAsync(string userEmail, Address newAddress)
        {
            var user = await _userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == userEmail);

            if (user == null)
                throw new AuthenticationException("User not found");

            if (user.Address == null)
            {
                user.Address = newAddress;
            }
            else
            {
                user.Address.UpdateFrom(newAddress);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Problem updating user address");

            return user.Address!;
        }

        public async Task<(AppUser, IEnumerable<string>)> GetUserByEmailWithAddress(string userEmail)
        {
            var user = await _userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == userEmail);
            if (user == null)
                throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            if (user == null) throw new AuthenticationException("User not found");

            return (user,roles);
        }

        private RefreshToken GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }


        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            // Fix: Ensure RefreshTokens is not null before calling .Single(...)
            var refreshTokens = user.RefreshTokens ?? new List<RefreshToken>();
            var refreshToken = refreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens!.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
