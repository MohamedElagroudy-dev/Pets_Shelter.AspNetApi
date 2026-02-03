using Core.Constants;
using System;

namespace Ecom.Application.AdoptionApplications.DTOs
{
    public class AdoptionApplicationDetailsDto
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string AdminNotes { get; set; } = string.Empty;

        // Applicant Info
        public string ApplicantId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Address Info
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Animal Info
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string AnimalDescription { get; set; } = string.Empty;
        public string AnimalPictureUrl { get; set; } = string.Empty;
    }
}
