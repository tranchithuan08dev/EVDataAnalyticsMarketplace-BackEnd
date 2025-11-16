using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class PaymentController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public PaymentController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayments(CancellationToken ct = default)
        {
            var payments = await _servicesProvider.PaymentService.GetPendingPaymentsAsync(ct);
            return Ok(payments);
        }

        [HttpPost("distribute/{paymentId}")]
        public async Task<IActionResult> DistributePayment([FromRoute] Guid paymentId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.PaymentService.DistributeRevenueAsync(paymentId, ct);
            if (!result)
            {
                return BadRequest("Cannot distribute revenue. Recheck PaymentId/Provider.OrganizationId or payment already distributed.");
            }

            return Ok();
        }
    }
}
