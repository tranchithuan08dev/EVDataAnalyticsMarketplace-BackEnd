namespace EV.AdminService.API.DTOs.DataModels
{
    public class ModerationDetailDTO
    {
        public Guid DatasetId { get; set; }
        public Guid DatasetVersionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VersionLabel { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string SampleFileUri { get; set; } = string.Empty;
        public List<DataQualityFlagDTO> Flags { get; set; } = new List<DataQualityFlagDTO>();
    }
}
