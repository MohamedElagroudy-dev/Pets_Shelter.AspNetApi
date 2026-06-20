using Core.Entities.Animal;
using Ecom.Application.FosterAnimals.DTOs;
using System.Linq;

namespace Ecom.Application.FosterAnimals.Mappings
{
    public static class FosterAnimalMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/animal-default.jpg";

        public static FosterAnimalDTO ToDto(this FosterAnimal animal)
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

            return new FosterAnimalDTO
            {
                Id = animal.Id,
                Name = animal.Name,
                Description = animal.Description,
                AgeYears = animal.AgeYears,
                Size = animal.Size.ToString(),
                WeightKg = animal.WeightKg,
                Gender = animal.Gender.ToString(),
                PetTypeId = animal.PetTypeId,
                PetTypeName = animal.PetType?.Name ?? string.Empty,
                Photos = photos,
                CreatedAt = animal.CreatedAt,
                AnimalsFriendlyLevel = animal.Temperament?.AnimalsFriendlyLevel ?? 1,
                ChildrenFriendlyLevel = animal.Temperament?.ChildrenFriendlyLevel ?? 1,
                HouseTrainedLevel = animal.Temperament?.HouseTrainedLevel ?? 1,
                FosterStartDate = animal.FosterStartDate,
                FosterEndDate = animal.FosterEndDate,
                FosterDurationDays = animal.FosterDurationDays,
                FosterNotes = animal.FosterNotes,
                Status = animal.Status.ToString(),
                IsUrgent = animal.IsUrgent
            };
        }

        public static FosterAnimal ToEntity(this AddFosterAnimalDTO dto)
        {
            return new FosterAnimal
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
                },
                FosterStartDate = dto.FosterStartDate,
                FosterEndDate = dto.FosterEndDate,
                FosterNotes = dto.FosterNotes,
                Status = dto.Status,
                IsUrgent = dto.IsUrgent
            };
        }

        public static void UpdateEntity(this FosterAnimal animal, UpdateFosterAnimalDTO dto)
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

            animal.FosterStartDate = dto.FosterStartDate;
            animal.FosterEndDate = dto.FosterEndDate;
            animal.FosterNotes = dto.FosterNotes;
            animal.Status = dto.Status;
            animal.IsUrgent = dto.IsUrgent;
        }
        public static FosterAnimalWithUserDTO ToWithUserDto(this FosterAnimal animal)
        {
            var animalDto = animal.ToDto();

            FosterAnimalWithUserDTO.UserSummary? userDto = null;
            if (animal.Fosterer != null)
            {
                userDto = new FosterAnimalWithUserDTO.UserSummary
                {
                    Id = animal.Fosterer.Id,
                    UserName = animal.Fosterer.UserName ?? string.Empty,
                    Email = animal.Fosterer.Email ?? string.Empty,
                    FirstName = animal.Fosterer.FirstName ?? string.Empty,
                    LastName = animal.Fosterer.LastName ?? string.Empty,
                    PictureUrl = animal.Fosterer.PictureUrl ?? string.Empty,
                    PhoneNumber = animal.Fosterer.PhoneNumber ?? string.Empty
                };
            }

            return new FosterAnimalWithUserDTO { Animal = animalDto, User = userDto };
        }
    }
}
