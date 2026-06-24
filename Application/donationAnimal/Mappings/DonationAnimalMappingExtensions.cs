using Application.donationAnimal.DTOs;
using Core.Constants;
using Core.Entities.Animal;
using Stripe.Terminal;
using System.Reflection.Metadata;


namespace Application.donationAnimal.Mappings
{
    public static class DonationAnimalMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/animal-default.jpg";
        private const string DefaultUserPicturePath = "/Images/Defult/DefultUserPic.jpeg";

        public static DonationAnimalDTO ToDto(this DonationAnimal animal)
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

            var donations = animal.Donations?.Select( d => new DonationDTO
            {
                Id = d.Id,
                DonationAnimalId = d.DonationAnimalId,
                Amount = d.Amount,
                Message = d.Message,
                DonatedAt = d.DonatedAt,
                DonorName = d.IsAnonymous
                    ? "Anonymous"
                    : d.Donor == null
                        ? "Anonymous"
                        : !string.IsNullOrWhiteSpace(d.DonorName)
                            ? d.DonorName
                            : $"{d.Donor.FirstName} {d.Donor.LastName}".Trim(),
                DonorPictureUrl = d.IsAnonymous
                    ? DefaultUserPicturePath
                    : d.DonorProfilePicture ?? DefaultUserPicturePath
            }).ToList() ?? new List<DonationDTO>();

            return new DonationAnimalDTO
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

                GoalAmount = animal.GoalAmount,
                CollectedAmount = animal.CollectedAmount,
                RemainingAmount = animal.RemainingAmount,
                ProgressPercentage = animal.ProgressPercentage,
                DonationStatus = animal.DonationStatus.ToString(),

                Donations = donations
            };
        }

        public static DonationAnimal ToEntity(this AddDonationAnimalDTO dto)
        {
            return new DonationAnimal
            {
                Name = dto.Name,
                Description = dto.Description,
                AgeYears = dto.AgeYears,
                Size = dto.Size,
                WeightKg = dto.WeightKg,
                Gender = dto.Gender,
                PetTypeId = dto.PetTypeId,
                GoalAmount = dto.GoalAmount,
                CollectedAmount = 0,
                DonationStatus = DonationStatus.Available,
                Temperament = new AnimalTemperament
                {
                    AnimalsFriendlyLevel = dto.AnimalsFriendlyLevel,
                    ChildrenFriendlyLevel = dto.ChildrenFriendlyLevel,
                    HouseTrainedLevel = dto.HouseTrainedLevel
                }
            };
        }

        public static void UpdateEntity(this DonationAnimal animal, UpdateDonationAnimalDTO dto)
        {
            animal.Name = dto.Name;
            animal.Description = dto.Description;
            animal.AgeYears = dto.AgeYears;
            animal.Size = dto.Size;
            animal.WeightKg = dto.WeightKg;
            animal.Gender = dto.Gender;
            animal.PetTypeId = dto.PetTypeId;
            animal.GoalAmount = dto.GoalAmount;
            animal.DonationStatus = dto.DonationStatus;

            if (animal.Temperament == null)
                animal.Temperament = new AnimalTemperament();

            animal.Temperament.AnimalsFriendlyLevel = dto.AnimalsFriendlyLevel;
            animal.Temperament.ChildrenFriendlyLevel = dto.ChildrenFriendlyLevel;
            animal.Temperament.HouseTrainedLevel = dto.HouseTrainedLevel;
        }
    }
}
