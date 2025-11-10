namespace EV.DataConsumerService.API.Models.DTOs
{
   public class DatasetSearchRequestDto
    {
        // Tham số tìm kiếm chung (tiêu đề, mô tả)
        public string Q { get; set; } = null; 
        
        // Bộ lọc danh mục
        public string Category { get; set; } = null; 

        // Bộ lọc khu vực (Region)
        public string Region { get; set; } = null; 

        // Bộ lọc loại xe (VehicleTypes)
        public string VehicleType { get; set; } = null; 

        // Bộ lọc giá (tối thiểu)
        public decimal? MinPrice { get; set; } = null; 

        // Bộ lọc giá (tối đa)
        public decimal? MaxPrice { get; set; } = null; 

        // Phân trang
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 25;
    }

    /// <sum
}
