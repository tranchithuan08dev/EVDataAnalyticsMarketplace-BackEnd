using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EV.DataConsumerService.API.Data.Repositories
{
    public class DatasetRepository : IDatasetRepository
    {

        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public DatasetRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        public IQueryable<Dataset> FindAllPublicDatasets()
        {
            return _context.Datasets
            .Where(d => d.Status == "approved" && d.Visibility == "public")
            .Include(d => d.DatasetVersions)
            .AsNoTracking(); 
        }

        

        public IQueryable<Dataset> GetFullPublicDatasetsQuery()
        {
            return _context.Datasets
             .Include(d => d.DatasetVersions)
             .Where(d => d.Visibility == "public" && d.Status == "approved");
        }
    }
}
