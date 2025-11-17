using EV.AdminService.API.DTOs.DataModels;
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

        public async Task<Payment?> GetPaymentByReferenceAsync(string referenceCode, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TransactionReference == referenceCode, ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(CancellationToken ct = default)
        {
            var payments = await _dbSet
                .AsNoTracking()
                .Where(p => p.MarketplaceFee == null)
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId,
                    PurchaseId = p.PurchaseId,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt,
                    IsDistributed = p.MarketplaceFee != null
                })
                .ToListAsync(ct)
                .ConfigureAwait(false);

            return payments;
        }

        public async Task<bool> DistributeRevenueAsync(Guid paymentId, CancellationToken ct = default)
        {
            var payment = await _dbSet
                .Include(p => p.Purchase)
                    .ThenInclude(pc => pc.DatasetVersion)
                        .ThenInclude(dv => dv.Dataset)
                            .ThenInclude(d => d.Provider)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, ct)
                .ConfigureAwait(false);

            if (payment == null)
            {
                return false;
            }

            if (payment.MarketplaceFee != null)
            {
                return false;
            }

            Guid providerOrgId = Guid.Empty;
            if (payment.Purchase?.DatasetVersion?.Dataset?.Provider?.OrganizationId != null)
            {
                providerOrgId = payment.Purchase.DatasetVersion.Dataset.Provider.OrganizationId;
            }
            else if (payment.SubscriptionId.HasValue)
            {
                var subscription = await _context.Subscriptions
                    .AsNoTracking()
                    .Include(s => s.Dataset)
                        .ThenInclude(d => d.Provider)
                    .FirstOrDefaultAsync(s => s.SubscriptionId == payment.SubscriptionId.Value, ct)
                    .ConfigureAwait(false);

                if (subscription?.Dataset?.Provider?.OrganizationId != null)
                {
                    providerOrgId = subscription.Dataset.Provider.OrganizationId;
                }
            }

            if (providerOrgId == Guid.Empty)
            {
                return false;
            }

            await using var tx = await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
            try
            {
                var procResult = await _context.Procedures.usp_DistributePaymentAsync(paymentId, providerOrgId, 20.0m, cancellationToken: ct).ConfigureAwait(false);

                if (procResult <= 0)
                {
                    await tx.RollbackAsync(ct).ConfigureAwait(false);
                    return false;
                }

                payment.MarketplaceFee = Math.Round(payment.Amount * 0.20m, 4);
                payment.PaidToProvider = Math.Round(payment.Amount - payment.MarketplaceFee.Value, 4);

                await SaveChangesAsync(ct).ConfigureAwait(false);

                await tx.CommitAsync(ct).ConfigureAwait(false);
                return true;
            }
            catch
            {
                await tx.RollbackAsync(ct).ConfigureAwait(false);
                throw;
            }
        }
    }
}
