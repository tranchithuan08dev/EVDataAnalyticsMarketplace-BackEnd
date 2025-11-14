using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.Entites;
using Microsoft.EntityFrameworkCore;

namespace EV.DataProviderService.API.Data.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly EvdataAnalyticsMarketplaceDbContext _context;

    public ProviderRepository(EvdataAnalyticsMarketplaceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
    {
        return await _context.Providers
            .Include(p => p.Organization)
            .Include(p => p.Datasets)
                .ThenInclude(d => d.DatasetVersions)
            .ToListAsync();
    }

    public async Task<Provider?> GetProviderByIdAsync(Guid providerId)
    {
        return await _context.Providers
            .Include(p => p.Organization)
            .Include(p => p.Datasets)
            .FirstOrDefaultAsync(p => p.ProviderId == providerId);
    }

    public async Task<Provider?> GetProviderWithDetailsAsync(Guid providerId)
    {
        return await _context.Providers
            .Include(p => p.Organization)
            .Include(p => p.Datasets)
                .ThenInclude(d => d.DatasetVersions)
            .FirstOrDefaultAsync(p => p.ProviderId == providerId);
    }
}

