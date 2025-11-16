namespace EV.DataProviderService.API.Models.DTOs
{
    public class DatasetDetailFullDto
    {
        // Thông tin Dataset (đã đầy đủ như yêu cầu trước)
        public Guid DatasetId { get; set; }
        public string Title { get; set; }
        public string LongDescription { get; set; }
        public string DataTypes { get; set; }
        public string Region { get; set; }
        public string BatteryTypes { get; set; }
        public string LicenseType { get; set; }
        public string Visibility { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin Provider và Organization
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCountry { get; set; }
        public bool IsProviderVerified { get; set; }

        /// <summary>
        /// Danh sách tất cả các phiên bản (Versions) của Dataset này.
        /// </summary>
        public IEnumerable<DatasetVersionDetailDto> Versions { get; set; } = new List<DatasetVersionDetailDto>();
    }
}
