using Microsoft.AspNetCore.Http;

namespace Application.Account.DTOs
{
    public class UpdatePictureDto
    {
        public required IFormFile Picture { get; set; }
    }
}
