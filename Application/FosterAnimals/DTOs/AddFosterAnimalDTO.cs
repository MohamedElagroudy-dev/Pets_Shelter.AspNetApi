using System;
using Microsoft.AspNetCore.Http;
using Core.Constants;

namespace Ecom.Application.FosterAnimals.DTOs
{
    public record AddFosterAnimalDTO
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public double AgeYears { get; init; }
        public AnimalSize Size { get; init; }
        public double WeightKg { get; init; }
        public Gender Gender { get; init; }
        public int PetTypeId { get; init; }
        public IFormFileCollection? Photos { get; init; }
        public int AnimalsFriendlyLevel { get; init; }
        public int ChildrenFriendlyLevel { get; init; }
        public int HouseTrainedLevel { get; init; }
        public DateTime? FosterStartDate { get; init; }
        public DateTime? FosterEndDate { get; init; }
        public string? FosterNotes { get; init; }
        public FosterStatus Status { get; init; } = FosterStatus.Available;
        public bool IsUrgent { get; init; }
    }
}
