using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;

namespace EV.AdminService.API.Repositories.Implements
{
    public class AnonymizationLogRepository : CRUDRepository<AnonymizationLog>
    {
        public AnonymizationLogRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }
    }
}
