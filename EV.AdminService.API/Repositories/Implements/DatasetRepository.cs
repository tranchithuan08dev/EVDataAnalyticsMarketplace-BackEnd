using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class DatasetRepository : CRUDRepository<Dataset>
    {
        public DatasetRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<object?> GetPendingDatasetAsync(CancellationToken ct = default)
        {
            return await _context.Datasets.AsNoTracking().Where(d => d.Status == "Pending")
                                .Include(d => d.Provider)
                                .ThenInclude(p => p.Organization)
                                .Select(d => new
                                {
                                    d.DatasetId,
                                    d.Title,
                                    d.ShortDescription,
                                    d.Category,
                                    d.Visibility,
                                    d.CreatedAt,
                                    Provider = new
                                    {
                                        d.Provider.ProviderId,
                                        Organization = new
                                        {
                                            d.Provider.OrganizationId,
                                            d.Provider.Organization.Name,
                                        }
                                    }
                                })
                                .ToListAsync(ct)
                                .ConfigureAwait(false);
        }

        public async Task<bool> SetStatusAsync(Guid datasetId, string status, CancellationToken ct = default)
        {
            var dataset = await _dbSet.FindAsync(new object[] { datasetId }, ct).AsTask().ConfigureAwait(false);
            if (dataset == null)
            {
                return false;
            }

            dataset.Status = status;
            dataset.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
    }
}
