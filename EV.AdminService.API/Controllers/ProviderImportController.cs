using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderImportController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public ProviderImportController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpPost("new-dataset")]
        [RequestSizeLimit(5_368_709_120)] // 5 GB
        [RequestFormLimits(MultipartBodyLengthLimit = 5_368_709_120)]
        public async Task<IActionResult> ImportDataset([FromForm] ImportDatasetRequest request, CancellationToken ct)
        {
            if (request.MetadataFile == null || request.DataFile == null)
            {
                return BadRequest("Cần cả 2 file: 'metadataFile' (xlsx) và 'dataFile' (csv).");
            }

            var orgIdClaim = User.FindFirstValue("OrganizationId");
            if (!Guid.TryParse(orgIdClaim, out var providerId))
            {
                return Unauthorized("Token không hợp lệ hoặc không chứa OrganizationId.");
            }

            try
            {
                var newDatasetId = await _servicesProvider.ProviderImportService.ImportNewDatasetAsync(request.MetadataFile, request.DataFile, providerId, ct);

                return Ok(new
                {
                    Message = "Tải lên thành công! Dữ liệu của bạn đang được xử lý bởi AI (kiểm duyệt & định giá).",
                    DatasetId = newDatasetId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình import.", Error = ex.Message });
            }
        }
    }
}
