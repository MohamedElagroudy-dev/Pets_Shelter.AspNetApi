using Application.UserDonations.DTOs;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserDonations.Services
{
    public interface IDonationService
    {
        Task<string> CreateDonationPaymentAsync(CreateDonationPaymentDto dto);
        Task HandleDonationCompleted(Session session);
    }
}
