using Application.Admin.DTO;
using Core.Entities;

namespace Application.Admin.Mappings
{
    public static class UserMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/DefultUserPic.jpeg";
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
                PictureUrl = user.PictureUrl,
                PersonalPicture = string.IsNullOrWhiteSpace(user.PictureUrl) ? DefaultImagePath : user.PictureUrl,
            };
        }
    }
}
