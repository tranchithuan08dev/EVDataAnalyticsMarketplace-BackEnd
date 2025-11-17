using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class DatasetRepository : CRUDRepository<Dataset>
    {
        public DatasetRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Dataset>> GetPendingDatasetsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.Status == "pending")
                .Include(d => d.Provider.Organization)
                .Include(d => d.DatasetVersions)
                .ThenInclude(dv => dv.DataQualityFlags)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
    }
}
