using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class DatasetVersionRepository : CRUDRepository<DatasetVersion>
    {
        public DatasetVersionRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<DatasetVersion?> GetPendingVersion(CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(v => v.Dataset.Status == "pending" && !v.DataQualityFlags.Any()).FirstOrDefaultAsync(ct).ConfigureAwait(false);
        }

        public async Task<DatasetVersion?> GetDatasetVersionAsync(Guid datasetVersionId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dv => dv.Dataset.Provider.Organization)
                .Include(dv => dv.DataQualityFlags)
                .FirstOrDefaultAsync(dv => dv.DatasetVersionId == datasetVersionId, ct)
                .ConfigureAwait(false);
        }
    }
}
