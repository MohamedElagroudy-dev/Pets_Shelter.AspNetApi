using Application.Admin.DTO;
using Core.Entities;

namespace Application.Admin.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserDto ToDto(this AppUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PictureUrl = user.PictureUrl
            };
        }
    }
}
