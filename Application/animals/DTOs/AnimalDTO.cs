using System;
using System.Collections.Generic;

namespace Ecom.Application.Animals.DTOs
{
    public record AnimalDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public double AgeYears { get; init; }
        public string Size { get; init; } = string.Empty;
        public double WeightKg { get; init; }
        public string Gender { get; init; } = string.Empty;
        public bool IsAdopted { get; init; }
        public int PetTypeId { get; init; }
        public string PetTypeName { get; init; } = string.Empty;

        public List<PhotoDTO> Photos { get; init; } = new();
        public DateTime CreatedAt { get; init; }

        // Temperament levels (1..5)
        public int AnimalsFriendlyLevel { get; init; }
        public int ChildrenFriendlyLevel { get; init; }
        public int HouseTrainedLevel { get; init; }
    }
}
