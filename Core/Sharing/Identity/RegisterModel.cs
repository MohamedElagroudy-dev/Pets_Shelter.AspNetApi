using System.ComponentModel.DataAnnotations;

namespace Core.Sharing.Identity
{
    public class RegisterModel
    {
        [Required, StringLength(100)]
        public required string FirstName { get; set; }

        [Required, StringLength(100)]
        public required string LastName { get; set; }

        [Required, StringLength(50)]
        public required string Username { get; set; }

        [Required, StringLength(128)]
        public required string Email { get; set; }

        [Required, StringLength(256)]
        public required string Password { get; set; }
    }
}