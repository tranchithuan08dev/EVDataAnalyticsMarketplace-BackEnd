namespace EV.AdminService.API.DTOs.DataModels
{
    public class PaymentDTO
    {
        public Guid PaymentId { get; set; }
        public Guid? PurchaseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public bool IsDistributed { get; set; } //MarketplaceFee != null
    }
}
