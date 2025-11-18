using EV.AdminService.API.DTOs.DataModels;
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

        public async Task<bool> HasActiveSubscriptionAsync(Guid organizationId, Guid datasetId, CancellationToken ct)
        {
            return await _dbSet.AsNoTracking()
                .AnyAsync(s => s.ConsumerOrgId == organizationId &&
                               s.DatasetId == datasetId &&
                               s.Active == true &&
                               (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow), ct)
                .ConfigureAwait(false);
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

        public async Task<IEnumerable<SubscriptionDetailDTO>> GetActiveSubscriptionsAsync(CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(s => s.Active)
                .Include(s => s.ConsumerOrg)
                .Include(s => s.Dataset)
                .Select(s => new SubscriptionDetailDTO
                {
                    SubscriptionId = s.SubscriptionId,
                    ConsumerOrgId = s.ConsumerOrgId,
                    ConsumerOrgName = s.ConsumerOrg.Name,
                    DatasetId = s.DatasetId,
                    DatasetTitle = s.Dataset.Title,
                    StartedAt = s.StartedAt,
                    ExpiresAt = s.ExpiresAt,
                    RecurringPrice = s.RecurringPrice,
                    Active = s.Active
                })
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
    }
}
