using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class ProviderRepository : CRUDRepository<Provider>
    {
        public ProviderRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<Provider?> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrganizationId == organizationId, ct)
                .ConfigureAwait(false);
        }
    }
}
