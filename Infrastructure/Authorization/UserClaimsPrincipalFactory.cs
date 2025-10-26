using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Restaurants.Infrastructure.Authorization
{
    public class UserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public UserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var id = await GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                id.AddClaim(new Claim("first_name", user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                id.AddClaim(new Claim("last_name", user.LastName));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                // Add both for safety
                id.AddClaim(new Claim(ClaimTypes.Email, user.Email)); 
                id.AddClaim(new Claim("email", user.Email));          //custom claim
            }

            return new ClaimsPrincipal(id);
        }
    }
}
