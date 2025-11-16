using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PolicyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AccessPolicy>> GetAllPoliciesAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.AccessPolicyRepository.GetAllAsync(true, ct).ConfigureAwait(false);
        }

        public async Task<AccessPolicy?> GetPolicyByIdAsync(int policyId, CancellationToken ct = default)
        {
            return await _unitOfWork.AccessPolicyRepository.GetByIdAsync(ct, policyId).ConfigureAwait(false);
        }

        public async Task<AccessPolicy> CreatePolicyAsync(CreateAccessPolicyRequest request, CancellationToken ct = default)
        {
            var newPolicy = new AccessPolicy
            {
                Name = request.Name,
                Description = request.Description,
                AllowedUse = request.AllowedUse,
                ExpiresInDays = request.ExpiresInDays
            };
            await _unitOfWork.AccessPolicyRepository.CreateAsync(newPolicy, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return newPolicy;
        }

        public async Task<bool> UpdatePolicyAsync(int policyId, UpdateAccessPolicyRequest request, CancellationToken ct = default)
        {
            var policy = await _unitOfWork.AccessPolicyRepository.GetByIdAsync(ct, policyId).ConfigureAwait(false);
            if (policy == null) return false;

            policy.Name = request.Name;
            policy.Description = request.Description;
            policy.AllowedUse = request.AllowedUse;
            policy.ExpiresInDays = request.ExpiresInDays;

            await _unitOfWork.AccessPolicyRepository.UpdateAsync(policy, ct).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeletePolicyAsync(int policyId, CancellationToken ct = default)
        {
            var policy = await _unitOfWork.AccessPolicyRepository.GetByIdAsync(ct, policyId).ConfigureAwait(false);
            if (policy == null) return false;

            var isPolicyInUse = await _unitOfWork.DatasetVersionRepository.AnyAsync(dv => dv.AccessPolicyId == policyId, ct);
            if (isPolicyInUse)
            {
                return false;
            }

            await _unitOfWork.AccessPolicyRepository.DeleteAsync(policy, ct).ConfigureAwait(false);
            return true;
        }
    }
}
