namespace EV.DataProviderService.API.Models.DTOs
{
    public class DatasetProviderListDto
    {
        public Guid DatasetId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Category { get; set; }
        public string Region { get; set; }
        public string Status { get; set; } // pending, approved, rejected
        public DateTime CreatedAt { get; set; }
    }
}
