using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.DTO
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string PictureUrl { get; set; } = string.Empty;

        public string Role { get; set; } = default!;
    }
}
