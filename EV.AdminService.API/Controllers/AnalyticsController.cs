using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public AnalyticsController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("popular-datasets")]
        public async Task<IActionResult> GetPopularDatasets(CancellationToken ct = default)
        {
            var datasets = await _servicesProvider.AnalyticsService.GetPopularDatasetsAsync(ct);
            return Ok(datasets);
        }

        [HttpGet("trend-reports")]
        public async Task<IActionResult> GetAITrendReports(CancellationToken ct = default)
        {
            var reports = await _servicesProvider.AnalyticsService.GetAITrendReportsAsync(ct);
            return Ok(reports);
        }

        [HttpGet("forecast/demand")]
        public async Task<IActionResult> GetDemandForecast([FromQuery] int days = 30, CancellationToken ct = default)
        {
            if (days <= 0 || days > 365)
            {
                return BadRequest("Days must be between 1 and 365.");
            }

            var combinedReport = await _servicesProvider.AnalyticsService.GetDemandForecastAsync(days, ct);
            return Ok(combinedReport);
        }
    }
}
