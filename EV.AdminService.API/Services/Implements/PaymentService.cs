using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using System.Globalization;
using System.Web;

namespace EV.AdminService.API.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponseDTO> CreatePaymentRequestAsync(CreatePaymentRequestDTO request, Guid consumerOrgId, CancellationToken ct)
        {
            decimal price = 0;
            Guid newPaymentId;

            using (var transaction = await _unitOfWork.BeginTransactionAsync(ct).ConfigureAwait(false))
            {
                var newPayment = new Payment
                {
                    Amount = 0,
                    Currency = "VND",
                    PaidToProvider = null,
                    MarketplaceFee = null,
                    PaidAt = DateTime.UtcNow,
                    PaymentGateway = "Sepay",
                };

                if (request.DatasetVersionId.HasValue)
                {
                    var version = await _unitOfWork.DatasetVersionRepository.GetByIdAsync(ct, request.DatasetVersionId.Value).ConfigureAwait(false);
                    if (version == null)
                    {
                        throw new KeyNotFoundException("Dataset Version không tồn tại.");
                    }

                    price = version.PricePerDownload ?? 0;

                    var newPurchase = new Purchase
                    {
                        ConsumerOrgId = consumerOrgId,
                        DatasetVersionId = version.DatasetVersionId,
                        Price = price,
                        Currency = "VND",
                        PurchasedAt = DateTime.UtcNow,
                    };

                    await _unitOfWork.PurchaseRepository.CreateAsync(newPurchase, ct).ConfigureAwait(false);

                    newPayment.PurchaseId = newPurchase.PurchaseId;
                    newPayment.SubscriptionId = null;
                    newPaymentId = newPayment.PaymentId;
                }
                else if (request.DatasetId.HasValue)
                {
                    var latestVersion = await _unitOfWork.DatasetVersionRepository.GetLatestVersionByDatasetIdAsync(request.DatasetId.Value, ct).ConfigureAwait(false);
                    if (latestVersion == null)
                    {
                        throw new KeyNotFoundException("Dataset không tồn tại phiên bản nào.");
                    }

                    price = latestVersion.PricePerGb ?? 0;

                    var newSub = new Subscription
                    {
                        ConsumerOrgId = consumerOrgId,
                        DatasetId = request.DatasetId.Value,
                        StartedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMonths(1),
                        RecurringPrice = price,
                        Currency = "VND",
                        Active = false
                    };

                    await _unitOfWork.SubscriptionRepository.CreateAsync(newSub, ct).ConfigureAwait(false);
                    newPayment.SubscriptionId = newSub.SubscriptionId;
                    newPayment.PurchaseId = null;
                    newPaymentId = newPayment.PaymentId;
                }
                else
                {
                    throw new InvalidOperationException("Yêu cầu không hợp lệ.");
                }

                if (price <= 0)
                {
                    throw new InvalidOperationException("Sản phẩm này không có giá.");
                }

                newPayment.Amount = price;
                await _unitOfWork.PaymentRepository.CreateAsync(newPayment, ct).ConfigureAwait(false);

                newPaymentId = newPayment.PaymentId;

                await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
                await transaction.CommitAsync(ct).ConfigureAwait(false);
            }

            var qrUrl = $"https://img.vietqr.io/image/tpbank-06598878601-compact2.jpg?amount={price}&addInfo={newPaymentId}&accountName=EVDATA%%MARKETPLACE";

            return new PaymentResponseDTO
            {
                PurchaseId = newPaymentId,
                PaymentUrl = qrUrl,
                Amount = price,
            };
        }

        public async Task<bool> DistributeRevenueAsync(Guid paymentId, CancellationToken ct = default)
        {
            return await _unitOfWork.PaymentRepository.DistributeRevenueAsync(paymentId, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.PaymentRepository.GetPendingPaymentsAsync(ct).ConfigureAwait(false);
        }

        public async Task<ProcessTransactionResult> ProcessSepayWebhookAsync(SepayWebhookPayload payload, CancellationToken ct)
        {
            var result = new ProcessTransactionResult
            {
                IsSuccessful = false,
            };

            try
            {
                var existingPayment = await _unitOfWork.PaymentRepository.GetPaymentByReferenceAsync(payload.ReferenceCode, ct).ConfigureAwait(false);

                if (existingPayment != null)
                {
                    result.Message = "Transaction already processed.";
                    result.IsSuccessful = true;
                    return result;
                }

                if (!Guid.TryParse(payload.Content, out var paymentId))
                {
                    result.Message = "Nội dung (Content) không chứa PaymentId (GUID) hợp lệ.";
                    return result;
                }

                var paymentToProcess = await _unitOfWork.PaymentRepository.GetByIdAsync(ct, paymentId).ConfigureAwait(false);

                if (paymentToProcess == null)
                {
                    result.Message = $"Không tìm thấy PaymentId {paymentId} trong hệ thống.";
                    return result;
                }

                if (payload.TransferAmount < paymentToProcess.Amount)
                {
                    result.Message = $"Không đủ số tiền. Yêu cầu {paymentToProcess.Amount}, nhận được {payload.TransferAmount}.";
                    return result;
                }

                paymentToProcess.TransactionReference = payload.ReferenceCode;
                paymentToProcess.PaidAt = ParseTransactionDate(payload.TransactionDate);
                paymentToProcess.PaymentGateway = payload.Gateway;

                await _unitOfWork.PaymentRepository.UpdateAsync(paymentToProcess, ct).ConfigureAwait(false);

                if (paymentToProcess.SubscriptionId.HasValue)
                {
                    var sub = await _unitOfWork.SubscriptionRepository.GetByIdAsync(ct, paymentToProcess.SubscriptionId.Value).ConfigureAwait(false);

                    if (sub != null && !sub.Active)
                    {
                        sub.Active = true;
                        await _unitOfWork.SubscriptionRepository.UpdateAsync(sub, ct).ConfigureAwait(false);
                    }
                }

                await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

                result.IsSuccessful = true;
                result.Message = "Giao dịch thành công.";
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "Lỗi hệ thống: " + ex.Message;
                return result;
            }
        }

        private DateTime ParseTransactionDate(string dateString)
        {
            try
            {
                return DateTime.ParseExact(
                    dateString,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture
                );
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }
    }
}
