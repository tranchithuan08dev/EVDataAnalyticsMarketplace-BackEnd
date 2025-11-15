using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Repositories
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public RevenueRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId, DateTime? startDate, DateTime? endDate)
        {
            var query = from p in _context.Purchases
                        join dv in _context.DatasetVersions on p.DatasetVersionId equals dv.DatasetVersionId
                        join d in _context.Datasets on dv.DatasetId equals d.DatasetId
                        join o in _context.Organizations on p.ConsumerOrgId equals o.OrganizationId
                        where d.ProviderId == providerId
                        select new { p, dv, d, o };

            if (startDate.HasValue) query = query.Where(x => x.p.PurchasedAt >= startDate.Value);
            if (endDate.HasValue) query = query.Where(x => x.p.PurchasedAt <= endDate.Value);

            var result = await query
                .GroupBy(x => 1)
                .Select(g => new RevenueReportDto
                {
                    ReportDate = DateTime.Now,
                    DownloadCount = g.Count(), 
                    TotalRevenue = g.Sum(x => x.p.Price), 
                    ConsumerDetails = string.Join(", ", g.Select(x => x.o.Name)) 
                })
                .FirstOrDefaultAsync();

            return result ?? new RevenueReportDto();
        }
    }
}