using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserDonations.DTOs
{
    public class CreateDonationPaymentDto
    {
        public int DonationAnimalId { get; set; }

        public decimal Amount { get; set; }

        public string? Message { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
