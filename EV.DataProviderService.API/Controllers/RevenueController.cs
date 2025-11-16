using Microsoft.AspNetCore.Mvc;
using EV.DataProviderService.API.Service;

namespace EV.DataProviderService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService;

        public RevenueController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        [HttpGet("{providerId}")]
        public async Task<IActionResult> GetRevenueReport(Guid providerId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var report = await _revenueService.GetRevenueReportAsync(providerId, startDate, endDate);
            return Ok(report);
        }
    }
}