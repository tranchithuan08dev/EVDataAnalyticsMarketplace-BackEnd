using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;

namespace EV.AdminService.API.Repositories.Implements
{
    public class RevenueShareRepository : CRUDRepository<RevenueShare>
    {
        public RevenueShareRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }
    }
}
