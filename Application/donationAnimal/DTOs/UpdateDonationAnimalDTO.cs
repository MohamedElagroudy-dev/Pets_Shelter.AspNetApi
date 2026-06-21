using Core.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.donationAnimal.DTOs
{
    public class UpdateDonationAnimalDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AgeYears { get; set; }
        public AnimalSize Size { get; set; }
        public double WeightKg { get; set; }
        public Gender Gender { get; set; }
        public int PetTypeId { get; set; }

        public int AnimalsFriendlyLevel { get; set; }
        public int ChildrenFriendlyLevel { get; set; }
        public int HouseTrainedLevel { get; set; }

        public decimal GoalAmount { get; set; }
        public DonationStatus DonationStatus { get; set; }

        public IFormFileCollection? Photos { get; init; }
    }
}
