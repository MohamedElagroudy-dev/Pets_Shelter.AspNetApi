using System;
using System.Collections.Generic;
using Core.Constants;

namespace Ecom.Application.FosterAnimals.DTOs
{
    public record FosterAnimalDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public double AgeYears { get; init; }
        public string Size { get; init; } = string.Empty;
        public double WeightKg { get; init; }
        public string Gender { get; init; } = string.Empty;
        public int PetTypeId { get; init; }
        public string PetTypeName { get; init; } = string.Empty;
        public List<PhotoDTO> Photos { get; init; } = new();
        public DateTime CreatedAt { get; init; }
        public int AnimalsFriendlyLevel { get; init; }
        public int ChildrenFriendlyLevel { get; init; }
        public int HouseTrainedLevel { get; init; }
        public DateTime? FosterStartDate { get; init; }
        public DateTime? FosterEndDate { get; init; }
        public int? FosterDurationDays { get; init; }
        public string? FosterNotes { get; init; }
        public string Status { get; init; } = string.Empty;
        public bool IsUrgent { get; init; }
    }
}
