using Core.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<(string paymentIntentId, string clientSecret)> CreateOrUpdatePaymentIntent(string? existingIntentId, long total);
        Task<string> RefundPayment(string paymentIntentId);
    }
}
