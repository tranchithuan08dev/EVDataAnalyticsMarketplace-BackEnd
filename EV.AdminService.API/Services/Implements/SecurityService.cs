using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SecurityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ApiKeyDTO>> GetActiveApiKeyAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.ApiKeyRepository.GetActiveApiKeysAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> RevokeApiKeyAsync(Guid apiKeyId, CancellationToken ct = default)
        {
            var key = await _unitOfWork.ApiKeyRepository.GetByIdAsync(ct, apiKeyId).ConfigureAwait(false);
            if (key == null)
            {
                return false;
            }

            key.Revoked = true;
            await _unitOfWork.ApiKeyRepository.UpdateAsync(key, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
    }
}
