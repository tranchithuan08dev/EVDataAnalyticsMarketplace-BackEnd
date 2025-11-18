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

        [HttpPost("create")]
        [RequestSizeLimit(5_368_709_120)]
        [RequestFormLimits(MultipartBodyLengthLimit = 5_368_709_120)]
        public async Task<IActionResult> CreateDataset([FromForm] CreateDatasetRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Token không hợp lệ hoặc không chứa OrganizationId.");
            }

            var user = await _servicesProvider.UserService.GetUserByIdAsync(userId, ct).ConfigureAwait(false);

            if (user == null || user.OrganizationId == null)
            {
                return Unauthorized("Không tìm thấy thông tin tổ chức (Organization) liên kết với tài khoản của bạn.");
            }

            var organization = await _servicesProvider.OrganizationService.GetOrganizationByIdAsync(user.OrganizationId.Value, ct).ConfigureAwait(false);

            if (organization == null || organization.Provider == null)
            {
                return Unauthorized("Không tìm thấy tổ chức (Organization) Provider của bạn.");
            }

            var providerId = organization.Provider.ProviderId;

            try
            {
                var newDatasetId = await _servicesProvider.ProviderImportService.CreateDatasetAsync(request, providerId, ct);

                return Ok(new
                {
                    Message = "Tạo dataset thành công! Dữ liệu đang được AI xử lý (kiểm duyệt & định giá).",
                    DatasetId = newDatasetId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi máy chủ.", Error = ex.Message });
            }
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

            //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!Guid.TryParse(userIdClaim, out var userId))
            //{
            //    return Unauthorized("Token không hợp lệ hoặc không chứa OrganizationId.");
            //}

            //var user = await _servicesProvider.UserService.GetUserByIdAsync(userId, ct).ConfigureAwait(false);

            //if (user == null || user.OrganizationId == null)
            //{
            //    return Unauthorized("Không tìm thấy thông tin tổ chức (Organization) liên kết với tài khoản của bạn.");
            //}

            //var organization = await _servicesProvider.OrganizationService.GetOrganizationByIdAsync(user.OrganizationId.Value, ct).ConfigureAwait(false);

            //if (organization == null || organization.Provider == null)
            //{
            //    return Unauthorized("Không tìm thấy tổ chức (Organization) Provider của bạn.");
            //}

            var providerId = Guid.Parse("8d1e6cec-3210-4e53-891e-188c326738f9");//organization.Provider.ProviderId;

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
