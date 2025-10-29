namespace EV.AdminService.API.DTOs.DataModels
{
    public class AnalysisReportDTO
    {
        public Guid AnalysisId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReportUri { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
