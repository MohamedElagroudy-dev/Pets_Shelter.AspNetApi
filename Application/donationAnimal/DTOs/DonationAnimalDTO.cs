using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.donationAnimal.DTOs
{
    public class DonationAnimalDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AgeYears { get; set; }
        public string Size { get; set; } = null!;
        public double WeightKg { get; set; }
        public string Gender { get; set; } = null!;
        public int PetTypeId { get; set; }
        public string PetTypeName { get; set; } = string.Empty;
        public List<PhotoDTO> Photos { get; set; } = new();
        public DateTime CreatedAt { get; set; }

        public int AnimalsFriendlyLevel { get; set; }
        public int ChildrenFriendlyLevel { get; set; }
        public int HouseTrainedLevel { get; set; }

        // Donation specific
        public decimal GoalAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public double ProgressPercentage { get; set; }
        public string DonationStatus { get; set; } = null!;

        // List of donations for this animal
        public List<DonationDTO> Donations { get; set; } = new();
    }
}
