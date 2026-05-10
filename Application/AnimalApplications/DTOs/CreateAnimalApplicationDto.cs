using Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecom.Application.AnimalApplications.DTOs
{
    public class CreateAnimalApplicationDto
    {
        [Required]
        public int AnimalId { get; set; }

        // ApplicantInfo
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;

        // AddressInfo
        [Required] public string Country { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string ZipCode { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;

        // HouseholdInfo
        [Required] public string HouseholdDetails { get; set; } = string.Empty;

        // PetCareInfo
        [Required] public string ResponsiblePerson { get; set; } = string.Empty;
        [Required] public string AdoptionReason { get; set; } = string.Empty;
        [Required] public string AloneTimeDetails { get; set; } = string.Empty;
        [Required] public string LivingEnvironment { get; set; } = string.Empty;

        // Preferences
        public bool Dog { get; set; }
        public bool Cat { get; set; }
        public bool Bird { get; set; }
        public bool Lizard { get; set; }
        public bool Rabbit { get; set; }
        public bool Other { get; set; }
        public bool HouseTrained { get; set; }
        public bool Declawed { get; set; }
        public bool Young { get; set; }
        public bool MultiplePets { get; set; }
        public bool SpecialConsiderations { get; set; }

        // Agreement
        [Required]
        public bool Accepted { get; set; }

        [JsonIgnore]
        public ApplicationType ApplicationType { get; set; } = ApplicationType.Adoption;
    }
}
