using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Animal
{
    public class DonationAnimal : BaseAnimal
    {
        public decimal GoalAmount { get; set; }
        public decimal CollectedAmount { get; set; } = 0;
        public decimal RemainingAmount => GoalAmount - CollectedAmount;
        public double ProgressPercentage => GoalAmount == 0 ? 0 : (double)(CollectedAmount / GoalAmount) * 100;
        public DonationStatus DonationStatus { get; set; } = DonationStatus.Available;

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
