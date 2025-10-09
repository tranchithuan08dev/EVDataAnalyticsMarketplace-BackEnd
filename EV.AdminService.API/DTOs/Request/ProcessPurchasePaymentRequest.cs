namespace EV.AdminService.API.DTOs.Request
{
    public class ProcessPurchasePaymentRequest
    {
        public Guid PurchaseId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? PaymentGateway { get; set; }
        public string? TransactionReference { get; set; }
    }
}
