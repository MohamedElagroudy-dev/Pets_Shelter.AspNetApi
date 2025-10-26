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
        [EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(256)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required, StringLength(256)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
