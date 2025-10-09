using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly Counter _paymentsProcessed;
        public DebugController(Counter paymentsProcessed)
        {
            _paymentsProcessed = paymentsProcessed;
        }

        [HttpPost("increment-payment")]
        public IActionResult IncrementPaymentMetric()
        {
            _paymentsProcessed.Inc();
            return Ok(new { message = "payments_processed_total incremented" });
        }
    }
}
