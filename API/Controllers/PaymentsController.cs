using Application.Orders.Services;
using Application.Payment.DTOs;
using Application.Payment.Services;
using Application.SignalR;
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

        public PaymentsController(IPaymentAppService paymentService,
            IUnitOfWork unit,
            IConfiguration config,
            IHubContext<NotificationHub> hubContext,
            IOrderService orderService)
        {
            _paymentService = paymentService;
            _unit = unit;
            _whSecret = config["StripeSettings:WhSecret"]!;
            _hubContext = hubContext;
            _orderService = orderService;
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

                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data.");
                }

                await _orderService.HandlePaymentIntentSucceeded(intent);

                return Ok();
            }
            catch (StripeException )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
            }
            catch (Exception )
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
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


        [HttpPost("checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession(
            CreateSessionDto dto)
        {
            var url = await _paymentService
                .CreateDonationCheckoutSession(dto.Amount);

            return Ok(new
            {
                CheckoutUrl = url
            });
        }

    }

}
