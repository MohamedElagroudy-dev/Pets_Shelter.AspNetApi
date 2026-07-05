using Core.Entities;
using Core.Entities.Cart;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
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
                    Currency = "egp",
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
        public async Task<string>CreateDonationCheckoutSessionAsync(int donationId,decimal amount)
        {
            var options = new SessionCreateOptions
            {
                    Mode = "payment",

                    SuccessUrl =
                        "https://petopia.0xcode7.xyz/sponsor",

                    CancelUrl =
                        "https://petopia.0xcode7.xyz/sponsor",

                    Metadata = new Dictionary<string, string>
                    {
                        { "Type", "Donation" },

                        { "DonationId", donationId.ToString() }
                    },

                    LineItems =
                    [
                        new SessionLineItemOptions
                        {
                            Quantity = 1,

                            PriceData =
                                new SessionLineItemPriceDataOptions
                                {
                                    Currency = "egp",

                                    UnitAmount =
                                        (long)(amount * 100),

                                    ProductData =
                                        new SessionLineItemPriceDataProductDataOptions
                                        {
                                            Name = $"Donation #{donationId}"
                                        }
                                }
                        }
                    ]
            };

            var service = new SessionService();

            var session =
                await service.CreateAsync(options);

            return session.Url!;
        }
    }
}
