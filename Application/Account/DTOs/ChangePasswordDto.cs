using System.ComponentModel.DataAnnotations;

namespace Application.Account.DTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
    }
}
