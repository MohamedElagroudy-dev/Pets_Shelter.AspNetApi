using Core.Entities.Animal;
using Ecom.Application.Animals.DTOs;
using System.Linq;
using Application.Admin.DTO;

namespace Ecom.Application.Animals.Mappings
{
    public static class AnimalMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/animal-default.jpg";

        public static AnimalDTO ToDto(this AdoptionAnimal animal)
        {
            var photos = animal.Photos?.Select(p => new PhotoDTO
            {
                Id = p.Id,
                AnimalId = p.AnimalId,
                ImageUrl = p.ImageUrl
            }).ToList() ?? new List<PhotoDTO>();

            if (photos.Count == 0)
            {
                photos.Add(new PhotoDTO { ImageUrl = DefaultImagePath });
            }

            return new AnimalDTO
            {
                Id = animal.Id,
                Name = animal.Name,
                Description = animal.Description,
                AgeYears = animal.AgeYears,
                Size = animal.Size.ToString(),
                WeightKg = animal.WeightKg,
                Gender = animal.Gender.ToString(),
                IsAdopted = animal.IsAdopted,
                PetTypeId = animal.PetTypeId,
                PetTypeName = animal.PetType?.Name ?? string.Empty,
                Photos = photos,
                CreatedAt = animal.CreatedAt,
                AnimalsFriendlyLevel = animal.Temperament?.AnimalsFriendlyLevel ?? 1,
                ChildrenFriendlyLevel = animal.Temperament?.ChildrenFriendlyLevel ?? 1,
                HouseTrainedLevel = animal.Temperament?.HouseTrainedLevel ?? 1
            };
        }

        public static AnimalWithUserDTO ToWithUserDto(this AdoptionAnimal animal)
        {
            var animalDto = animal.ToDto();

            AnimalWithUserDTO.UserSummary? userDto = null;
            if (animal.Adopter != null)
            {
                userDto = new AnimalWithUserDTO.UserSummary
                {
                    Id = animal.Adopter.Id,
                    UserName = animal.Adopter.UserName ?? string.Empty,
                    Email = animal.Adopter.Email ?? string.Empty,
                    FirstName = animal.Adopter?.FirstName,
                    LastName = animal.Adopter?.LastName,
                    PictureUrl = animal.Adopter?.PictureUrl ?? string.Empty,
                    PhoneNumber = animal.Adopter?.PhoneNumber?? string.Empty
                };
            }

            return new AnimalWithUserDTO { Animal = animalDto, User = userDto };
        }

        public static AdoptionAnimal ToEntity(this AddAnimalDTO dto)
        {
            return new AdoptionAnimal
            {
                Name = dto.Name,
                Description = dto.Description,
                AgeYears = dto.AgeYears,
                Size = dto.Size,
                WeightKg = dto.WeightKg,
                Gender = dto.Gender,
                PetTypeId = dto.PetTypeId,
                Temperament = new AnimalTemperament
                {
                    AnimalsFriendlyLevel = dto.AnimalsFriendlyLevel,
                    ChildrenFriendlyLevel = dto.ChildrenFriendlyLevel,
                    HouseTrainedLevel = dto.HouseTrainedLevel
                }
            };
        }

        public static void UpdateEntity(this AdoptionAnimal animal, UpdateAnimalDTO dto)
        {
            animal.Name = dto.Name;
            animal.Description = dto.Description;
            animal.AgeYears = dto.AgeYears;
            animal.Size = dto.Size;
            animal.WeightKg = dto.WeightKg;
            animal.Gender = dto.Gender;
            animal.PetTypeId = dto.PetTypeId;

            if (animal.Temperament == null)
                animal.Temperament = new AnimalTemperament();

            animal.Temperament.AnimalsFriendlyLevel = dto.AnimalsFriendlyLevel;
            animal.Temperament.ChildrenFriendlyLevel = dto.ChildrenFriendlyLevel;
            animal.Temperament.HouseTrainedLevel = dto.HouseTrainedLevel;
        }
    }
}
