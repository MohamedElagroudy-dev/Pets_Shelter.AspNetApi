using Application.Subscriptions.Services;
using Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscriptions([FromQuery] PlanType? type)
    {
        if (!type.HasValue)
        {
            return BadRequest("Subscription type is required.");
        }

        var subscriptions = await _subscriptionService.GetSubscriptionsByTypeAsync(type.Value);
        return Ok(subscriptions);
    }
}