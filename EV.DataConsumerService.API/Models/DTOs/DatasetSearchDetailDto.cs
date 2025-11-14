namespace EV.DataConsumerService.API.Models.DTOs
{
    public class DatasetSearchDetailDto
    {
        // Thông tin Dataset (Từ bảng Datasets)
        public Guid DatasetId { get; set; }
        public string Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Category { get; set; }
        public string? Region { get; set; }

        // Thuộc tính cần thêm: VehicleTypes
        public string? VehicleTypes { get; set; }

        // Thông tin Dataset Version (Từ bảng DatasetVersions)
        public Guid DatasetVersionId { get; set; }
        public string VersionLabel { get; set; }
        public string FileFormat { get; set; }
        public decimal? PricePerDownload { get; set; } // Tương ứng với PricePerDownload trong DV
    }
}
