namespace EV.DataProviderService.API.Models.DTOs
{
    public class ProviderDatasetSummaryDto
    {
        public Guid DatasetId { get; set; }
        public string Title { get; set; }
        public string? ShortDescription { get; set; }

        // --- CÁC TRƯỜNG ĐƯỢC BỔ SUNG ---
        public string? LongDescription { get; set; }
        public string? DataTypes { get; set; }
        public string? Region { get; set; }
        public string? BatteryTypes { get; set; }
        public string LicenseType { get; set; } // SQL có Default, nhưng nên là string? nếu có thể null
                                                // ------------------------------

        public string? Category { get; set; }
        public string? VehicleTypes { get; set; }
        public string Visibility { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // --- TRƯỜNG BỔ SUNG ---
        public DateTime? UpdatedAt { get; set; }
        // ----------------------
    }
}
