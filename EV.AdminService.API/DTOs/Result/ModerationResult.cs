namespace EV.AdminService.API.DTOs.Result
{
    public class ModerationResult
    {
        public Guid DatasetId { get; set; }
        public double Score { get; set; } // 0..1
        public List<string> Labels { get; set; } = new();
        public string? Explanation { get; set; }
        public DateTime AnalyzedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
