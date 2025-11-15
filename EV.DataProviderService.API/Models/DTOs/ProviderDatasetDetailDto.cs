namespace EV.DataProviderService.API.Models.DTOs
{
    public class ProviderDatasetDetailDto
    {
        /// <summary>
        /// Thông tin chi tiết về Provider.
        /// </summary>
        public ProviderDetailDto Provider { get; set; }

        /// <summary>
        /// Danh sách các Dataset mà Provider này đã tạo.
        /// </summary>
        public IEnumerable<ProviderDatasetSummaryDto> Datasets { get; set; } = new List<ProviderDatasetSummaryDto>();
    }
}
