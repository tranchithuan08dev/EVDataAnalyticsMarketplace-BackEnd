namespace EV.DataConsumerService.API.Models.DTOs
{
   
        // DTO cho Dataset Version (phiên bản dữ liệu)
        public class DatasetVersionDto
        {
            public Guid DatasetVersionId { get; set; }
            public string VersionLabel { get; set; }
            public string FileFormat { get; set; }
            public decimal? PricePerDownload { get; set; }
            public bool SubscriptionRequired { get; set; }
            public string StorageUri { get; set; }
        }

        // DTO chính để lấy Dataset cùng với các Versions
        public class DatasetFullDetailDto
        {
            public Guid DatasetId { get; set; }
            public string Title { get; set; }
            public string ShortDescription { get; set; } // Dữ liệu này có thể NULL
            public string Category { get; set; }
            public string Region { get; set; }
            public string VehicleTypes { get; set; }
            public string Status { get; set; }

            // Danh sách Versions liên kết
            public List<DatasetVersionDto> Versions { get; set; } = new List<DatasetVersionDto>();
        }
    }

