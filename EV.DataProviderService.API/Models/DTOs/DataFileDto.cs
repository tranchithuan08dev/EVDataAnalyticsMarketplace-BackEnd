namespace EV.DataProviderService.API.Models.DTOs
{
    public class DataFileDto
    {
        public Guid DataFileId { get; set; }

        public Guid DatasetVersionId { get; set; }
        public string FileName { get; set; }

        public string FileUri { get; set; }
        public long? FileSizeBytes { get; set; } 
        public string? Checksum { get; set; }
      
    }
}
