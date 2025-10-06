using System.ComponentModel.DataAnnotations;

namespace EV.DataConsumerService.API.Models.DTOs
{
    public class DatasetSearchResultDto
    {
        [Key]
        public Guid DatasetId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Category { get; set; }
        public string Region { get; set; }

        public string FileFormat { get; set; }
        public decimal? MinPricePerDownload { get; set; }
        public bool HasSubscriptionOption { get; set; }
    }
}
