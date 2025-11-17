namespace EV.DataProviderService.API.Models.DTOs
{
    public class AnonymizationResultDto
    {
        public Guid AnonymizationId { get; set; }
        public Guid DatasetVersionId { get; set; }
        public string Method { get; set; }
        public DateTime PerformedAt { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}