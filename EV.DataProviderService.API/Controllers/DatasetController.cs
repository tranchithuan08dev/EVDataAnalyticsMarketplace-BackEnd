using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;
using EV.DataProviderService.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EV.DataProviderService.API.Controllers
{
    // [Authorize(Roles = "Provider")] // Cần xác thực vai trò
    [Route("[controller]")]
    [ApiController]
    public class DatasetsController : ControllerBase
    {
        private readonly IDatasetService _service;
        // SỬ DỤNG PROVIDER ID THỰC TẾ ĐÃ TỒN TẠI TRONG DB CỦA BẠN ĐỂ TEST
        private readonly Guid _providerId = new Guid("54CFF31D-FCBD-4C37-B3FE-02CB9DF8EE6D"); // THAY BẰNG ID THỰC TẾ

        public DatasetsController(IDatasetService service)
        {
            _service = service;
        }

        // GET /api/provider/datasets
        /// <summary>Lấy danh sách tất cả Datasets do Provider này quản lý.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDatasets()
        {
            // Giả định _providerId được lấy từ token hoặc context
            var datasets = await _service.GetAllDatasetsAsync(_providerId);

            if (datasets == null || !datasets.Any())
            {
                return NotFound("No datasets found for this provider.");
            }

            return Ok(datasets);
        }
    }
}