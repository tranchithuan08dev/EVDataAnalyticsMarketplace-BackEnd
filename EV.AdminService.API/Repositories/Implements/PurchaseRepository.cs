using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class PurchaseRepository : CRUDRepository<Purchase>
    {
        public PurchaseRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<bool> HasPurchasedVersionAsync(Guid organizationId, Guid datasetVersionId, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking()
                .AnyAsync(p => p.ConsumerOrgId == organizationId && p.DatasetVersionId == datasetVersionId, ct)
                .ConfigureAwait(false);
        }

        public async Task<Dictionary<Guid, (Guid DatasetId, string Title, int PurchaseCount)>> GetPurchaseCountsAsync(CancellationToken ct = default)
        {
            var purchaseCounts = await _dbSet.AsNoTracking().Include(p => p.DatasetVersion.Dataset)
                            .GroupBy(p => new { p.DatasetVersion.Dataset.DatasetId, p.DatasetVersion.Dataset.Title })
                            .Select(g => new
                            {
                                g.Key.DatasetId,
                                g.Key.Title,
                                PurchaseCount = g.Count()
                            })
                            .ToDictionaryAsync(x => x.DatasetId, x => (x.DatasetId, x.Title, x.PurchaseCount), ct).ConfigureAwait(false);
            return purchaseCounts;
        }
    }
}
