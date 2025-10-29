using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class PaymentRepository : CRUDRepository<Payment>
    {
        public PaymentRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(CancellationToken ct = default)
        {
            var payments = await _dbSet.AsNoTracking().Where(p => p.MarketplaceFee == null)
                                    .Select(p => new PaymentDTO
                                    {
                                        PaymentId = p.PaymentId,
                                        PurchaseId = p.PurchaseId,
                                        Amount = p.Amount,
                                        PaidAt = p.PaidAt,
                                        IsDistributed = false
                                    }).ToListAsync(ct).ConfigureAwait(false);

            return payments;
        }

        public async Task<bool> DistributeRevenueAsync(Guid paymentId, CancellationToken ct = default)
        {
            var providerOrgId = await _dbSet.AsNoTracking().Select(p => p.Purchase.DatasetVersion.Dataset.Provider.OrganizationId)
                                        .FirstOrDefaultAsync(ct).ConfigureAwait(false);

            if (providerOrgId == Guid.Empty)
            {
                return false;
            }

            var result = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                EXEC DistributeRevenue 
                    @PaymentId = {paymentId}, 
                    @ProviderOrganizationId = {providerOrgId},
                    @MarketplacePercentage = 20.0").ConfigureAwait(false);

            return result > 0;
        }
    }
}
