namespace EV.DataProviderService.API.Models.DTOs
{
    public class DatasetVersionDetailDto
    {
        public Guid DatasetVersionId { get; set; }
        public string VersionLabel { get; set; }
        public string FileFormat { get; set; }

    
        public DateTime CreatedAt { get; set; } 
        public long? FilesizeBytes { get; set; }
        public string? StorageUri { get; set; }
        public bool IsAnalyzed { get; set; }
        public string? AnalysisReportUri { get; set; }
        public string? SampleUri { get; set; }
        public decimal? PricePerDownload { get; set; }
        public decimal? PricePerGB { get; set; } 
        public bool HasSubscriptionOption { get; set; } 
        public int? AccessPolicyId { get; set; }
        public string? LicenseText { get; set; }
     

        /// <summary>
        /// Danh sách các file dữ liệu thuộc phiên bản này.
        /// </summary>
        public IEnumerable<DataFileDto> DataFiles { get; set; } = new List<DataFileDto>();
    }
}
