using System.ComponentModel.DataAnnotations;

namespace Application.Account.DTOs
{
    public class ProfileUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressDto? Address { get; set; }
    }
}
