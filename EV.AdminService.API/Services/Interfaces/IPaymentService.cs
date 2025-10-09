namespace EV.AdminService.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Guid?> ProcessPurchasePaymentAsync(Guid purchaseId, decimal amount, string currency, string paymentGateway, string transactionReference, CancellationToken ct = default);
    }
}
