using Ecom.Application.FosterAnimals.DTOs;

namespace Ecom.Application.FosterAnimals.DTOs
{
    public record FosterAnimalWithUserDTO
    {
        public FosterAnimalDTO Animal { get; init; } = default!;
        public UserSummary? User { get; init; }

        public record UserSummary
        {
            public string Id { get; init; } = string.Empty;
            public string UserName { get; init; } = string.Empty;
            public string Email { get; init; } = string.Empty;
            public string PhoneNumber { get; init; } = string.Empty;
            public string? FirstName { get; init; }
            public string? LastName { get; init; }
            public string PictureUrl { get; init; } = string.Empty;
        }
    }
}
