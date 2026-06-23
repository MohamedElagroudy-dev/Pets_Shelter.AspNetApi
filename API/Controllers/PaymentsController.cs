using Application.Orders.Services;
using Application.Payment.DTOs;
using Application.Payment.Services;
using Application.SignalR;
using Application.UserDonations.Services;
using Core.Constants;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stripe;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentAppService _paymentService;
        private readonly IUnitOfWork _unit;
        private readonly string _whSecret;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IOrderService _orderService;
        private readonly IDonationService _donationService;

        public PaymentsController(IPaymentAppService paymentService,
            IUnitOfWork unit,
            IConfiguration config,
            IHubContext<NotificationHub> hubContext,
            IOrderService orderService,
            IDonationService donationService)
        {
            _paymentService = paymentService;
            _unit = unit;
            _whSecret = config["StripeSettings:WhSecret"]!;
            _hubContext = hubContext;
            _orderService = orderService;
            _donationService = donationService;
        }

      
        [Authorize(Roles = UserRoles.Customer)]
        [HttpPost("{cartId}")]
        public async Task<ActionResult> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await _paymentService.CreateOrUpdatePaymentIntent(cartId);
            if (cart == null) return BadRequest("Problem with your cart on the API");
            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var methods = await _paymentService.GetDeliveryMethodsAsync();
            return Ok(methods);
        }



        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = ConstructStripeEvent(json);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;

                    if (intent != null)
                    {
                        await _orderService.HandlePaymentIntentSucceeded(intent);
                    }
                }
                else if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    if (session != null)
                    {
                        await _donationService.HandleDonationCompleted(session);
                    }
                }

                return Ok();
            }
            catch (StripeException)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Webhook error");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
            }
            catch (Exception )
            {
                throw new StripeException("Invalid signature");
            }
        }


        //[HttpPost("checkout-session")]
        //public async Task<IActionResult> CreateCheckoutSession(
        //    CreateSessionDto dto)
        //{
        //    var url = await _paymentService
        //        .CreateDonationCheckoutSession(dto);

        //    return Ok(new
        //    {
        //        CheckoutUrl = url
        //    });
        //}

    }

}
