namespace EV.DataConsumerService.API.Models.DTOs
{
    public class SubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid ApiKeyId { get; set; }
        public string NewApiKey { get; set; } // Khóa API mới được tạo (chỉ hiển thị một lần)
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; }
    }
}
