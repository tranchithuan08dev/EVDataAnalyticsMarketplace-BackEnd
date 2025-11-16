using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface ISecurityService
    {
        Task<IEnumerable<ApiKeyDTO>> GetActiveApiKeyAsync(CancellationToken ct = default);
        Task<bool> RevokeApiKeyAsync(Guid apiKeyId, CancellationToken ct = default);
    }
}
