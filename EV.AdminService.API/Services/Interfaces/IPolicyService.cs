using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Models;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IPolicyService
    {
        Task<IEnumerable<AccessPolicy>> GetAllPoliciesAsync(CancellationToken ct = default);
        Task<AccessPolicy?> GetPolicyByIdAsync(int policyId, CancellationToken ct = default);
        Task<AccessPolicy> CreatePolicyAsync(CreateAccessPolicyRequest request, CancellationToken ct = default);
        Task<bool> UpdatePolicyAsync(int policyId, UpdateAccessPolicyRequest request, CancellationToken ct = default);
        Task<bool> DeletePolicyAsync(int policyId, CancellationToken ct = default);
    }
}
