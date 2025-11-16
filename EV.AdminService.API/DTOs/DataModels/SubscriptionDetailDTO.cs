namespace EV.AdminService.API.DTOs.DataModels
{
    public class SubscriptionDetailDTO
    {
        public Guid SubscriptionId { get; set; }
        public Guid ConsumerOrgId { get; set; }
        public string ConsumerOrgName { get; set; } = string.Empty;
        public Guid DatasetId { get; set; }
        public string DatasetTitle { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal RecurringPrice { get; set; }
        public bool Active { get; set; }
    }
}
