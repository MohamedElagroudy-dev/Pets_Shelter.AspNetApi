using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public enum DonationPaymentStatus
    {
        Pending = 0,   // created, waiting for Stripe
        Succeeded = 1, // webhook confirmed payment
        Failed = 2,
        Cancelled = 3
    }
}
