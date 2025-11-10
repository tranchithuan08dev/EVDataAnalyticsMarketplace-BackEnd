using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace EV.DataConsumerService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EVDatasetsController : ControllerBase
    {
        private readonly IDatasetService _datasetService;
        private readonly ILogger<EVDatasetsController> _logger; 

      
        public EVDatasetsController(IDatasetService datasetService, ILogger<EVDatasetsController> logger)
        {
            _datasetService = datasetService;
            _logger = logger;
        }

        [HttpGet()]
        [DisableRequestSizeLimit]
        public IActionResult GetFullDatasetList()
        {
            // 1. Ghi log trước khi thực hiện hành động chính
            _logger.LogInformation("Processing GET request for full dataset list.");

            try
            {
                // 2. Ghi log trước khi gọi Service
                _logger.LogDebug("Calling GetFullDatasetDetails() from dataset service.");

                var datasets = _datasetService.GetFullDatasetDetails();

                // 3. Ghi log thành công
                if (datasets != null)
                {
                    _logger.LogInformation("Successfully retrieved {DatasetCount} datasets.", datasets.Count);
                }
                else
                {
                    _logger.LogWarning("GetFullDatasetDetails returned null or empty list.");
                }

                return Ok(datasets);
            }
            catch (Exception ex)
            {
                // 4. Ghi log lỗi chi tiết (FATAL/ERROR)
                // LogError sẽ tự động ghi lại Stack Trace của ngoại lệ (ex)
                _logger.LogError(ex, "An unhandled exception occurred while retrieving the full dataset list.");

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DatasetSearchResultDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchDatasets([FromQuery] DatasetSearchRequestDto searchRequest)
        {
            // Logic kiểm tra tham số đầu vào cơ bản
            if (searchRequest.Page < 1 || searchRequest.PageSize < 1)
            {
                return BadRequest("Page và PageSize phải lớn hơn hoặc bằng 1.");
            }

            try
            {
                var results = await _datasetService.SearchDatasetsAsync(searchRequest);
                // Trả về 200 OK với kết quả
                return Ok(results);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (ví dụ: logger.LogError(ex, "Search failed"))
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server trong quá trình tìm kiếm dữ liệu.");
            }
        }
    }
}