using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;

        public AdminController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersByRole([FromQuery] string role, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(role)) return BadRequest("Query parameter 'role' is required.");

            var result = await _servicesProvider.UserService.GetUsersByRoleAsync(role, ct);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsers(CancellationToken ct = default)
        {
            var users = await _servicesProvider.UserService.GetAllUsersAsync(ct);
            return Ok(users);
        }

        [HttpPut("users/{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles([FromRoute] Guid id, [FromBody] List<string> roles, CancellationToken ct = default)
        {
            if (roles == null) return BadRequest("Roles required in body.");
            var ok = await _servicesProvider.UserService.UpdateUserRolesAsync(id, roles, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("users/{id}/active")]
        public async Task<IActionResult> SetUserActive([FromRoute] Guid id, [FromQuery] bool isActive, CancellationToken ct = default)
        {
            var ok = await _servicesProvider.UserService.SetUserActiveAsync(id, isActive, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders(CancellationToken ct = default)
        {
            var res = await _servicesProvider.UserService.GetProvidersAsync(ct);
            return Ok(res);
        }

        [HttpGet("consumers")]
        public async Task<IActionResult> GetConsumers(CancellationToken ct = default)
        {
            var res = await _servicesProvider.UserService.GetConsumersAsync(ct);
            return Ok(res);
        }

        [HttpGet("datasets/pending")]
        public async Task<IActionResult> GetPendingDatasets(CancellationToken ct = default)
        {
            var res = await _servicesProvider.DatasetService.GetPendingDatasetsAsync(ct);
            return Ok(res);
        }

        [HttpPut("datasets/{id}/status")]
        public async Task<IActionResult> SetDatasetStatus([FromRoute] Guid id, [FromQuery] string status, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(status)) return BadRequest("Query parameter 'status' is required.");
            var ok = await _servicesProvider.DatasetService.SetDatasetStatusAsync(id, status, ct);
            if (!ok) return BadRequest("Unable to update dataset status. Allowed: pending, approved, rejected.");
            return NoContent();
        }
    }
}
