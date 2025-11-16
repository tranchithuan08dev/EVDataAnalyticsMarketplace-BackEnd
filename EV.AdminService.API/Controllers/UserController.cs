using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "AdminOnly")]
    public class UserController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public UserController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersAsync(CancellationToken ct = default)
        {
            var users = await _servicesProvider.UserService.GetUsersAsync(ct).ConfigureAwait(false);
            return Ok(users);
        }

        [HttpPut("users/{userId}/status")]
        public async Task<IActionResult> SetUserStatus(Guid userId, [FromBody] UpdateUserStatusRequest request)
        {
            var result = await _servicesProvider.UserService.SetUserStatusAsync(userId, request.IsActive);
            if (!result) return NotFound("Người dùng không tìm thấy.");
            return Ok();
        }

        [HttpPut("users/{userId}/roles")]
        public async Task<IActionResult> UpdateUserRoles(Guid userId, [FromBody] UpdateUserRolesRequest request)
        {
            var result = await _servicesProvider.UserService.UpdateUserRolesAsync(userId, request.RoleIds);
            if (!result) return NotFound("Người dùng không tìm thấy.");
            return Ok();
        }

        [HttpPost("organizations/{organizationId}/verify")]
        public async Task<IActionResult> VerifyProvider(Guid organizationId)
        {
            var result = await _servicesProvider.UserService.VerifyProviderAsync(organizationId);
            if (!result) return NotFound("Tổ chức không tìm thấy hoặc không phải là nhà cung cấp.");
            return Ok();
        }
    }
}
