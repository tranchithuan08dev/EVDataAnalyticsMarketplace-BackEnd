namespace EV.AdminService.API.Services.Interfaces
{
    public interface IApiKeyService
    {
        Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default);
        Task<Guid> GetOrganizationIdForKeyAsync(string apiKey, CancellationToken ct = default);
    }
}
