using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Prometheus;

namespace EV.AdminService.API.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Counter _paymentsProcessed;
        private const decimal MarketplaceFeePercentage = 0.10m;
        public PaymentService(IUnitOfWork unitOfWork, Counter paymentsProcessed)
        {
            _unitOfWork = unitOfWork;
            _paymentsProcessed = paymentsProcessed;
        }
        public async Task<Guid?> ProcessPurchasePaymentAsync(Guid purchaseId, decimal amount, string currency, string paymentGateway, string transactionReference, CancellationToken ct = default)
        {
            if (amount <= 0)
            {
                return null;
            }

            await _unitOfWork.BeginTransactionAsync(ct).ConfigureAwait(false);

            try
            {
                var purchase = await _unitOfWork.PurchaseRepository.GetWithVersionAndDatasetAsync(purchaseId, ct).ConfigureAwait(false);
                if (purchase == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct).ConfigureAwait(false);
                    return null;
                }

                var providerOrgId = purchase.DatasetVersion?.Dataset?.Provider?.OrganizationId;
                if (providerOrgId == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct).ConfigureAwait(false);
                    return null;
                }

                var marpketplaceFee = Math.Round(amount * MarketplaceFeePercentage, 4);
                var paidToProvider = amount - marpketplaceFee;

                var payment = new Payment
                {
                    PurchaseId = purchaseId,
                    Amount = amount,
                    Currency = currency,
                    MarketplaceFee = marpketplaceFee,
                    PaidToProvider = paidToProvider,
                    PaymentGateway = paymentGateway,
                    TransactionReference = transactionReference,
                };

                await _unitOfWork.PaymentRepository.CreateAsync(payment, ct).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

                var revenue = new RevenueShare
                {
                    PaymentId = payment.PaymentId,
                    ToOrganizationId = providerOrgId.Value,
                    Amount = paidToProvider,
                    Currency = currency,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.RevenueShareRepository.CreateAsync(revenue, ct).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

                await _unitOfWork.CommitTransactionAsync(ct).ConfigureAwait(false);

                _paymentsProcessed.Inc();
                return payment.PaymentId;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(ct).ConfigureAwait(false);
                throw;
            }
        }
    }
}
