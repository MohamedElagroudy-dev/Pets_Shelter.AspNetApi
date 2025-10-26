using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Account
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }

    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor?.HttpContext?.User;
            if (user == null)
            {
                throw new InvalidOperationException("User context is not present");
            }

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = user.FindFirst("uid")?.Value 
                         ?? throw new InvalidOperationException("User Id not found in token");

            var email = user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                        ?? user.FindFirst(ClaimTypes.Email)?.Value
                        ?? throw new InvalidOperationException("Email not found in token");

            var roles = user.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value);

            var firstName = user.FindFirst("first_name")?.Value ?? string.Empty;
            var lastName = user.FindFirst("last_name")?.Value ?? string.Empty;

            var username = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;


            return new CurrentUser(userId, email, roles, firstName, lastName, username);
        }
    }
}
