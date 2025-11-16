namespace EV.DataProviderService.API.Models.DTOs
{
    public class AnonymizationRequestDto
    {
        public Guid DatasetVersionId { get; set; }
        public List<string> FieldsToAnonymize { get; set; } = new(); // e.g., ["Email", "IpAddress"]
        public string Method { get; set; } = "Hash"; // Hash, Mask, Delete, Replace
        public Guid PerformedBy { get; set; } // UserId
    }
}
