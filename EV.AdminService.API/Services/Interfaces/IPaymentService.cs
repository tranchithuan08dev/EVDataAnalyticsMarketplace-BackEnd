using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(CancellationToken ct = default);
        Task<bool> DistributeRevenueAsync(Guid paymentId, CancellationToken ct = default);
        Task<PaymentResponseDTO> CreatePaymentRequestAsync(CreatePaymentRequestDTO request, Guid consumerOrgId, CancellationToken ct);
        Task<ProcessTransactionResult> ProcessSepayWebhookAsync(SepayWebhookPayload payload, CancellationToken ct);
    }
}
