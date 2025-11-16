namespace EV.AdminService.API.DTOs.DataModels
{
    public class PopularDatasetDTO
    {
        public Guid DatasetId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PurchaseCount { get; set; }
        public int SubscriptionCount { get; set; }
        public int TotalInterest { get; set; }
    }
}
