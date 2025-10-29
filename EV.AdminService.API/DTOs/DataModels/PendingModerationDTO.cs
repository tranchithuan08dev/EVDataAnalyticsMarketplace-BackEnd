namespace EV.AdminService.API.DTOs.DataModels
{
    public class PendingModerationDTO
    {
        public Guid DatasetId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public int RedFlags { get; set; }
        public int YellowFlags { get; set; }
        public int GreenFlags { get; set; }
    }
}
