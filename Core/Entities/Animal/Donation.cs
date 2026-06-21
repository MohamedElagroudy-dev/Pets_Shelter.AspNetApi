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

        public decimal Amount { get; set; }
        public string? Message { get; set; }          // "Hope Finn finds a loving home soon!"
        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;
    }
}
