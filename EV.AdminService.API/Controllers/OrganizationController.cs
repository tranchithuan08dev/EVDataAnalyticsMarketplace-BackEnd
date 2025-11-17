using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public OrganizationController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrganizationsAsync(CancellationToken ct = default)
        {
            var organizations = await _servicesProvider.OrganizationService.GetOrganizationsAsync(ct).ConfigureAwait(false);
            return Ok(organizations);
        }
    }
}
