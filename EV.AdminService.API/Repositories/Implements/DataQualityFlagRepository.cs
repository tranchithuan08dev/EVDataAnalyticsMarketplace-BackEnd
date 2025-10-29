using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;

namespace EV.AdminService.API.Repositories.Implements
{
    public class DataQualityFlagRepository : CRUDRepository<DataQualityFlag>
    {
        public DataQualityFlagRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }
    }
}
