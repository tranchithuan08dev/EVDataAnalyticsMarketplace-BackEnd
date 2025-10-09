using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.Entites;
using Microsoft.EntityFrameworkCore;

namespace EV.DataProviderService.API.Data.Repositories
{
    public class DatasetRepository :  IDatasetRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public DatasetRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Dataset>> GetAllDatasetsByProviderIdAsync(Guid providerId)
        {
            // Truy vấn tất cả Datasets có ProviderId khớp
            return await _context.Datasets
                 //.Where(d => d.ProviderId == providerId)
                 //.OrderByDescending(d => d.CreatedAt)
                 .ToListAsync();
        }
    }
}
