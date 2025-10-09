using EV.AdminService.API.DTOs.Request;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public PaymentsController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> ProcessPurchasePayment([FromBody] ProcessPurchasePaymentRequest req, CancellationToken ct = default)
        {
            if (req == null) return BadRequest();
            var paymentId = await _servicesProvider.PaymentService.ProcessPurchasePaymentAsync(req.PurchaseId, req.Amount, req.Currency ?? "USD", req.PaymentGateway ?? "unknown", req.TransactionReference ?? string.Empty, ct);
            if (paymentId == null) return BadRequest("Unable to process payment.");
            return Ok(new { paymentId });
        }
    }
}
