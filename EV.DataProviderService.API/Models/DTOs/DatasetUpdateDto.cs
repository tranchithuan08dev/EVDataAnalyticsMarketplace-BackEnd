using System.ComponentModel.DataAnnotations;

namespace EV.DataProviderService.API.Models.DTOs
{
    public class DatasetUpdateDto
    {
        [Required]
        public Guid DatasetId { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string Category { get; set; }

        public string DataTypes { get; set; }

        public string Region { get; set; }

        public string VehicleTypes { get; set; }

        public string BatteryTypes { get; set; }
    }
}
