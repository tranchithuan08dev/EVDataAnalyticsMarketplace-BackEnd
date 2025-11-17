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

        public async Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId)
        {
            var datasets = await _context.Datasets
                .Where(d => d.ProviderId == providerId)
                .Include(d => d.DatasetVersions)
                    .ThenInclude(dv => dv.Purchases)
                .ToListAsync();

            var totalRevenue = datasets
                .SelectMany(d => d.DatasetVersions)
                .SelectMany(dv => dv.Purchases)
                .Sum(p => p.Price);

            var totalDownloads = datasets
                .SelectMany(d => d.DatasetVersions)
                .Sum(dv => dv.Purchases.Count);

            var recentDatasets = datasets
                .OrderByDescending(d => d.CreatedAt)
                .Take(10)
                .Select(d => new RevenueSummaryItemDto
                {
                    DatasetTitle = d.Title,
                    DownloadCount = d.DatasetVersions.Sum(dv => dv.Purchases.Count),
                    Revenue = d.DatasetVersions.SelectMany(dv => dv.Purchases).Sum(p => p.Price),
                    Status = d.Status == "approved" ? "Đã duyệt" : "Chờ duyệt"
                })
                .ToList();

            return new RevenueReportDto
            {
                TotalDownloadCount = totalDownloads,
                TotalRevenue = totalRevenue,
                TotalDatasets = datasets.Count,
                RecentDatasets = recentDatasets
            };
        }
    }
}