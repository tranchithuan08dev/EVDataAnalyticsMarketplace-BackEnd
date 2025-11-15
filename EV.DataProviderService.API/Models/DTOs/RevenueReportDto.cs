namespace EV.DataProviderService.API.Models.DTOs
{
    public class RevenueReportDto
    {
        public DateTime ReportDate { get; set; }
        public int DownloadCount { get; set; } 
        public decimal TotalRevenue { get; set; } 
        public string ConsumerDetails { get; set; } 
    }
}