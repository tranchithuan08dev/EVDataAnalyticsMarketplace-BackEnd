namespace EV.AdminService.API.DTOs.DataModels
{
    public class DataQualityFlagDTO
    {
        public long FlagId { get; set; }
        public string FlagType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ProcessedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
