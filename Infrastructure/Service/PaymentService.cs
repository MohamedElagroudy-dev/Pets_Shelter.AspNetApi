using Core.Entities;
using Core.Entities.Cart;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class PaymentService : IPaymentService
    {

        public PaymentService(IConfiguration config, ICartService cartService,
            IUnitOfWork unit)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        }

        public async Task<(string paymentIntentId, string clientSecret)> CreateOrUpdatePaymentIntent(string? existingIntentId, long total)
        {
            var service = new PaymentIntentService();

            if (string.IsNullOrEmpty(existingIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = total,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                var intent = await service.CreateAsync(options);
                return (intent.Id, intent.ClientSecret);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions { Amount = total };
                var intent = await service.UpdateAsync(existingIntentId, options);
                return (intent.Id, intent.ClientSecret);
            }
        }

        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions { PaymentIntent = paymentIntentId };
            var result = await refundService.CreateAsync(refundOptions);
            return result.Status;
        }
    }
}
