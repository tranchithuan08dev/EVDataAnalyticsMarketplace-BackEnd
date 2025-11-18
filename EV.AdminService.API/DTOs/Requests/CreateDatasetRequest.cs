using System.ComponentModel.DataAnnotations;

namespace EV.AdminService.API.DTOs.Requests
{
    public class CreateDatasetRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string VehicleTypes { get; set; } = string.Empty;
        [Required]
        public string VersionLabel { get; set; } = string.Empty;
        [Required]
        public IFormFile DataFile { get; set; } = null!;
    }
}
