using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;

        public SubscriptionController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSubscriptions(CancellationToken ct = default)
        {
            var subscriptions = await _servicesProvider.SubscriptionService.GetActiveSubscriptionsAsync(ct);
            return Ok(subscriptions);
        }

        [HttpPost("cancel/{subscriptionId}")]
        public async Task<IActionResult> CancelSubscription(Guid subscriptionId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.SubscriptionService.CancelSubscriptionAsync(subscriptionId, ct);
            if (!result)
            {
                return NotFound("Subscription not found or already inactive.");
            }
            return Ok("Subscription canceled successfully.");
        }
    }
}
