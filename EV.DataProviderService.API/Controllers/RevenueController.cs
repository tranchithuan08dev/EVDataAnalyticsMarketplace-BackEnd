using Microsoft.AspNetCore.Mvc;
using EV.DataProviderService.API.Service;
using System;

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

        [HttpGet("dashboard/{providerId}")]
        public async Task<IActionResult> GetRevenueReport(Guid providerId)
        {
            var report = await _revenueService.GetRevenueReportAsync(providerId);
            return Ok(report);
        }
    }
}