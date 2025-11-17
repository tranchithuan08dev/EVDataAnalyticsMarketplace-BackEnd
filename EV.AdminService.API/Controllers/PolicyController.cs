using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;

        public PolicyController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPolicies(CancellationToken ct = default)
        {
            var policies = await _servicesProvider.PolicyService.GetAllPoliciesAsync(ct);
            return Ok(policies);
        }

        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicyById(int policyId, CancellationToken ct = default)
        {
            var policy = await _servicesProvider.PolicyService.GetPolicyByIdAsync(policyId, ct);
            if (policy == null) return NotFound();
            return Ok(policy);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy([FromBody] CreateAccessPolicyRequest request, CancellationToken ct = default)
        {
            var policy = await _servicesProvider.PolicyService.CreatePolicyAsync(request, ct);
            return CreatedAtAction(nameof(GetPolicyById), new { policyId = policy.AccessPolicyId }, policy);
        }

        [HttpPut("{policyId}")]
        public async Task<IActionResult> UpdatePolicy(int policyId, [FromBody] UpdateAccessPolicyRequest request, CancellationToken ct = default)
        {
            var result = await _servicesProvider.PolicyService.UpdatePolicyAsync(policyId, request, ct);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{policyId}")]
        public async Task<IActionResult> DeletePolicy(int policyId, CancellationToken ct = default)
        {
            var result = await _servicesProvider.PolicyService.DeletePolicyAsync(policyId, ct);
            if (!result) return BadRequest("Policy not found or is currently in use by a DatasetVersion.");
            return NoContent();
        }
    }
}
