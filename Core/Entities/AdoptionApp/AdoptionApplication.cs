using Core.Constants;


namespace Core.Entities.AdoptionApp
{
    public class AdoptionApplication : BaseEntity
    {
        // Relations
        public int AnimalId { get; set; }
        public Core.Entities.Animal.Animal Animal { get; set; } = null!;


        public string ApplicantId { get; set; } = null!;
        public AppUser Applicant { get; set; } = null!;

        // Owned sections
        public ApplicantInfo ApplicantInfo { get; set; } = new();
        public AddressInfo AddressInfo { get; set; } = new();
        public HouseholdInfo HouseholdInfo { get; set; } = new();
        public PetCareInfo PetCareInfo { get; set; } = new();
        public AdoptionPreferences Preferences { get; set; } = new();
        public AgreementInfo Agreement { get; set; } = new();

        // Review
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public string? AdminNotes { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }
}
