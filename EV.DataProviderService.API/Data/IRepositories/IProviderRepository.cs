using EV.DataProviderService.API.Models.Entites;

namespace EV.DataProviderService.API.Data.IRepositories;

public interface IProviderRepository
{
    Task<IEnumerable<Provider>> GetAllProvidersAsync();
    Task<Provider?> GetProviderByIdAsync(Guid providerId);
    Task<Provider?> GetProviderWithDetailsAsync(Guid providerId);
}

