namespace EV.AdminService.API.DTOs.DataModels
{
    public class PaymentResponseDTO
    {
        public Guid PurchaseId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
