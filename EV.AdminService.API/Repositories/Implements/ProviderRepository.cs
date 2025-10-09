using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class ProviderRepository : CRUDRepository<Provider>
    {
        public ProviderRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<object?> GetProvider(CancellationToken ct = default)
        {
            return await _context.Providers
                    .AsNoTracking()
                    .Include(p => p.Organization)
                    .Select(p => new
                    {
                        p.ProviderId,
                        Organization = new { p.Organization.OrganizationId, p.Organization.Name, p.Organization.OrgType }
                    })
                    .ToListAsync(ct);
        }
    }
}
