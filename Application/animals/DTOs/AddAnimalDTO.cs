using Microsoft.AspNetCore.Http;
using Core.Constants;

namespace Ecom.Application.Animals.DTOs
{
    public record AddAnimalDTO
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public double AgeYears { get; init; }
        public AnimalSize Size { get; init; }
        public double WeightKg { get; init; }
        public Gender Gender { get; init; }
        public int PetTypeId { get; init; }
        public IFormFileCollection? Photos { get; init; }

        // Temperament levels (1..5)
        public int AnimalsFriendlyLevel { get; init; }
        public int ChildrenFriendlyLevel { get; init; }
        public int HouseTrainedLevel { get; init; }
    }
}
