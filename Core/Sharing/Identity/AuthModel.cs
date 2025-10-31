using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Sharing.Identity
{
    public class AuthModel
    {
        public string PictureUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
        //public DateTime ExpiresOn { get; set; }

        //[JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}