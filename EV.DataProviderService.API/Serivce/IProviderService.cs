using EV.DataProviderService.API.Models.DTOs;

namespace EV.DataProviderService.API.Service;

public interface IProviderService
{
    Task<IEnumerable<ProviderListDto>> GetAllProvidersAsync();
    Task<ProviderListDto?> GetProviderByIdAsync(Guid providerId);
}

