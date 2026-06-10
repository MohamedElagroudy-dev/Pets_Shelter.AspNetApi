using Core.Constants;
using System;

namespace Ecom.Application.AnimalApplications.DTOs
{
    public class AnimalApplicationDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string ApplicantId { get; set; } = string.Empty;
        public string ApplicantFirstName { get; set; } = string.Empty;
        public string ApplicantLastName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;

        public string ApplicantPicture { set; get; } = default!;
        public ApplicationStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }

        // Single picture URL for the animal
        public string AnimalPictureUrl { get; set; } = string.Empty;
    }
}
