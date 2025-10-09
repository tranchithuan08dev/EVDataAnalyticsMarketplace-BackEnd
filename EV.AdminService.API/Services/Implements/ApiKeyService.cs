using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApiKeyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> GetOrganizationIdForKeyAsync(string apiKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Guid.Empty;
            }

            var key = await _unitOfWork.ApiKeyRepository.GetByActiveKeyHashAsync(apiKey, ct).ConfigureAwait(false);
            if (key == null)
            {
                return Guid.Empty;
            }

            return key.OrganizationId;
        }

        public async Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            var key = await _unitOfWork.ApiKeyRepository.GetByActiveKeyHashAsync(apiKey, ct).ConfigureAwait(false);
            return key != null;
        }
    }
}
