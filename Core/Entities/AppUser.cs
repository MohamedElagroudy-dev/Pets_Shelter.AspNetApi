using Ecom.Core.Entities.Product;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Address? Address { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }

        public string PictureUrl { get; set; } = string.Empty;
    }
}
