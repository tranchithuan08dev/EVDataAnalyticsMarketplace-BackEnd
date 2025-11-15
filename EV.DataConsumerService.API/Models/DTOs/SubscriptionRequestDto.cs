using System.ComponentModel.DataAnnotations;

namespace EV.DataConsumerService.API.Models.DTOs
{
    public class SubscriptionRequestDto
    {
        [Required]
        public Guid ConsumerOrgId { get; set; } // Tổ chức đăng ký thuê bao

        [Required]
        public Guid DatasetId { get; set; } // Dataset được thuê bao

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá thuê bao định kỳ phải lớn hơn 0.")]
        public decimal RecurringPrice { get; set; } // Giá thuê bao định kỳ

        [Required]
        [Range(1, 365, ErrorMessage = "Thời hạn phải từ 1 đến 365 ngày.")]
        public int DurationDays { get; set; } = 30; // Thời hạn thuê bao (mặc định 30 ngày)
    }
}
