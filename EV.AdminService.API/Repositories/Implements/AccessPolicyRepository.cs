using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;

namespace EV.AdminService.API.Repositories.Implements
{
    public class AccessPolicyRepository : CRUDRepository<AccessPolicy>
    {
        public AccessPolicyRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }
    }
}
