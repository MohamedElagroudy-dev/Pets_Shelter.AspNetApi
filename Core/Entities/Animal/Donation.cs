using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Animal
{
    public class Donation : BaseEntity
    {
        public int DonationAnimalId { get; set; }
        [ForeignKey(nameof(DonationAnimalId))]
        public virtual DonationAnimal DonationAnimal { get; set; } = null!;

        public string DonorId { get; set; } = null!;
        public virtual AppUser Donor { get; set; } = null!;

        public bool IsAnonymous { get; set; } = false;

        public string? DonorName { get; set; }   
        public string? DonorProfilePicture { get; set; } 

        public decimal Amount { get; set; }
        public string? Message { get; set; }          
        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;

        public DonationPaymentStatus PaymentStatus { get; set; } = DonationPaymentStatus.Pending;
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
