using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http; // Cần thiết cho DisableRequestSizeLimit

namespace EV.DataConsumerService.API.Controllers
{
    // [ApiController]: Kích hoạt các quy ước API Controller (ví dụ: tự động trả về 400 Bad Request cho lỗi ModelState)
    [ApiController]

    // [Route("[controller]")] Định tuyến cơ bản: Controller name (EVDatasets)
    [Route("[controller]")]
    public class EVDatasetsController : ControllerBase // Kế thừa từ ControllerBase (lớp cơ sở cho API)
    {
        private readonly IDatasetService _datasetService;

        public EVDatasetsController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        /// <summary>
        /// Lấy tất cả các Datasets (công khai và đã duyệt) cùng với các Dataset Versions liên kết.
        /// Endpoint: /api/consumer/EVDatasets
        /// </summary>
        [HttpGet()]

        // [DisableRequestSizeLimit]: Bắt buộc để tránh lỗi JSON bị cắt do buffering của Gateway/Kestrel khi trả về dữ liệu lớn.
        [DisableRequestSizeLimit]
        public IActionResult GetFullDatasetList()
        {
            try
            {
                // Gọi Service để thực hiện Eager Loading và Mapping sang DTO
                var datasets = _datasetService.GetFullDatasetDetails();

                // Trả về kết quả HTTP 200 OK
                return Ok(datasets);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi cấp thấp (như lỗi kết nối DB, timeout) và trả về 500
                // Lỗi này xảy ra nếu chuỗi kết nối SQL Server vẫn sai (ví dụ: sử dụng localhost thay vì tên service Docker).
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}