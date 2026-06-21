using System;

namespace Application.donationAnimal.DTOs
{
    public class DonationDTO
    {
        public int Id { get; set; }
        public int DonationAnimalId { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Message { get; set; }
        public DateTime DonatedAt { get; set; }
    }
}
