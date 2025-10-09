using System.ComponentModel.DataAnnotations;

namespace EV.DataProviderService.API.Models.DTOs
{
    public class DatasetCreateDto
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        [Required]
        public string Category { get; set; } // e.g., "Driving Behavior"

        public string DataTypes { get; set; } // e.g., "gps,soc,charging_session"

        public string Region { get; set; } // e.g., "Europe"

        public string VehicleTypes { get; set; } // e.g., "BEV,PHEV"

        public string BatteryTypes { get; set; } // e.g., "Li-ion"

        // Ban đầu luôn là private, pending
        // public string Visibility { get; set; } 
        // public string Status { get; set; } 
    }
}
