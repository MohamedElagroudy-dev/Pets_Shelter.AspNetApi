using System.ComponentModel.DataAnnotations;

namespace Core.Sharing.Identity
{
    public class AddRoleModel
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}