using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public RoleController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId, CancellationToken ct = default)
        {
            var role = await _servicesProvider.RoleService.GetRoleByIdAsync(roleId, ct);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken ct = default)
        {
            var role = await _servicesProvider.RoleService.CreateRoleAsync(request, ct);
            return CreatedAtAction(nameof(GetRoleById), new { roleId = role.RoleId }, role);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId, [FromBody] UpdateRoleRequest request, CancellationToken ct = default)
        {
            var result = await _servicesProvider.RoleService.UpdateRoleAsync(roleId, request, ct);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.RoleService.DeleteRoleAsync(roleId, ct);
            if (!result) return BadRequest("Role not found or is in use.");
            return NoContent();
        }
    }
}
