using Core.Constants;
using System;

namespace Ecom.Application.AdoptionApplications.DTOs
{
    public class AdoptionApplicationDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string ApplicantId { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Single picture URL for the animal
        public string AnimalPictureUrl { get; set; } = string.Empty;
    }
}
