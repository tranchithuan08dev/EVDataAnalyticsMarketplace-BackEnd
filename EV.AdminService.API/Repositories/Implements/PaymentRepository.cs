using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;

namespace EV.AdminService.API.Repositories.Implements
{
    public class PaymentRepository : CRUDRepository<Payment>
    {
        public PaymentRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }
    }
}
