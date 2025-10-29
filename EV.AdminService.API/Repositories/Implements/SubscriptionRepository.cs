using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class SubscriptionRepository : CRUDRepository<Subscription>
    {
        public SubscriptionRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<Dictionary<Guid, int>> GetSubsciptionCountsAsync(CancellationToken ct = default)
        {
            var subCounts = await _dbSet.AsNoTracking().Include(s => s.Dataset)
                            .Where(s => s.Active)
                            .GroupBy(s => new { s.Dataset.DatasetId, s.Dataset.Title })
                            .Select(g => new
                            {
                                g.Key.DatasetId,
                                SubscriptionCount = g.Count()
                            })
                            .ToDictionaryAsync(x => x.DatasetId, x => x.SubscriptionCount, ct).ConfigureAwait(false);
            return subCounts;
        }
    }
}
