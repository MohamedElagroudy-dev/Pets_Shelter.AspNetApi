namespace Ecom.Application.Animals.DTOs
{
    public record PhotoDTO
    {
        public int Id { get; init; }
        public int AnimalId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
    }
}
