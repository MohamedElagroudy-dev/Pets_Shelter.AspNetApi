using System.ComponentModel.DataAnnotations;

namespace Core.Sharing.Identity
{
    public class TokenRequestModel
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}