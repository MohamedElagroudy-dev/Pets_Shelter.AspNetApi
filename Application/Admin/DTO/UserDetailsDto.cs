using System.Collections.Generic;
using Ecom.Application.Animals.DTOs;
using Ecom.Application.FosterAnimals.DTOs;

namespace Application.Admin.DTO
{
    public record UserDetailsDto
    {
        public UserDto User { get; init; } = default!;
        public List<AnimalDTO> AdoptedAnimals { get; init; } = new();
        public List<FosterAnimalDTO> FosteredAnimals { get; init; } = new();
    }
}
