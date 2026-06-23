using Application.UserDonations.DTOs;
using Application.UserDonations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationService _donationService;
        public DonationsController(IDonationService donationService)
        {
            _donationService = donationService;
        }
        [Authorize]
        [HttpPost("donation")]
        public async Task<IActionResult> CreateDonationPayment(CreateDonationPaymentDto dto)
        {
            var url =
                await _donationService
                    .CreateDonationPaymentAsync(dto);

            return Ok(new
            {
                CheckoutUrl = url
            });
        }
    }
}
