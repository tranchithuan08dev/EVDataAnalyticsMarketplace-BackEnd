namespace EV.DataProviderService.API.Models.DTOs
{
    public class RevenueSummaryItemDto
    {
        public string DatasetTitle { get; set; } = string.Empty;
        public int DownloadCount { get; set; }
        public decimal Revenue { get; set; }
        public string Status { get; set; } = "Chờ duyệt";
    }

    public class RevenueReportDto
    {
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public int TotalDownloadCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalDatasets { get; set; }
        public List<RevenueSummaryItemDto> RecentDatasets { get; set; } = new();
    }
}