namespace EV.AdminService.API.DTOs.DataModels
{
    public class DatasetVersionMetadataDTO
    {
        public Guid DatasetVersionId { get; set; }
        public string VersionLabel { get; set; } = string.Empty;
        public string FileFormat { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
