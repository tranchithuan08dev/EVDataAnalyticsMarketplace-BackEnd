using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class PurchaseRepository : CRUDRepository<Purchase>
    {
        public PurchaseRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<Purchase?> GetWithVersionAndDatasetAsync(Guid purchaseId, CancellationToken ct = default)
        {
            return await _context.Purchases.AsNoTracking()
                        .Include(p => p.DatasetVersion)
                        .ThenInclude(dv => dv.Dataset)
                        .ThenInclude(d => d.Provider)
                        .ThenInclude(p => p.Organization)
                        .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId, ct).ConfigureAwait(false);
        }
    }
}
