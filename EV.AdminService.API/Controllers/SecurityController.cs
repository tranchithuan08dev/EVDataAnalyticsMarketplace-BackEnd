using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class SecurityController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public SecurityController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveApiKeys(CancellationToken ct = default)
        {
            var apikeys = await _servicesProvider.SecurityService.GetActiveApiKeyAsync(ct).ConfigureAwait(false);
            return Ok(apikeys);
        }

        [HttpPost("revoke/{apiKeyId}")]
        public async Task<IActionResult> RevokeApiKey ([FromRoute] Guid apiKeyId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.SecurityService.RevokeApiKeyAsync(apiKeyId, ct).ConfigureAwait(false);
            if (!result)
            {
                return NotFound("Notfound APIKEYID.");
            }
            return Ok();
        }
    }
}
