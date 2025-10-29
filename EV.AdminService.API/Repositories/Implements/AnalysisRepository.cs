using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class AnalysisRepository : CRUDRepository<Analysis>
    {
        public AnalysisRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AnalysisReportDTO>> GetPublicOrRestrictedAnalysisAsync(CancellationToken ct = default)
        {
            var analyses = await _dbSet.AsNoTracking().Where(a => a.Visibility == "public" || a.Visibility == "restricted")
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new AnalysisReportDTO
                    {
                        AnalysisId = a.AnalysisId,
                        Title = a.Title,
                        Description = a.Description,
                        CreatedAt = a.CreatedAt,
                        ReportUri = a.ReportUri,
                    })
                    .ToListAsync(ct).ConfigureAwait(false);

            return analyses;
        }
    }
}
