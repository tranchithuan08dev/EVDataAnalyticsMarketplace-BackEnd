using System.ComponentModel.DataAnnotations;

namespace EV.DataConsumerService.API.Models.DTOs
{
    public class PurchaseRequestDto
    {
        [Required]
        public Guid BuyerUserId { get; set; } // Người dùng thực hiện mua

        [Required]
        public Guid BuyerOrgId { get; set; } // Tổ chức của người dùng

        [Required]
        public Guid DatasetVersionId { get; set; } // Phiên bản dữ liệu được mua

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; } // Giá được thanh toán

        public string? Currency { get; set; } = "USD";

        // Số ngày có quyền truy cập (nếu gói dữ liệu có thời hạn)
        public int? AccessDays { get; set; }
    }
}
