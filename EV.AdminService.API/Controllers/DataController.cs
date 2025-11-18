using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiKeyPolicy")]
    public class DataController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        private readonly ILogger<DataController> _logger;
        public DataController(IServicesProvider servicesProvider, ILogger<DataController> logger)
        {
            _servicesProvider = servicesProvider;
            _logger = logger;
        }

        [HttpGet("datasets/{datasetId}/versions")]
        public async Task<IActionResult> GetAvailableVersions([FromRoute] Guid datasetId, CancellationToken ct = default)
        {
            var organizationId = GetOrganizationIdFromToken();
            if (organizationId == Guid.Empty) return Unauthorized("Token API Key không hợp lệ.");

            try
            {
                var versions = await _servicesProvider.DataConsumerService
                    .GetDatasetVersionsAsync(organizationId, datasetId, ct);

                return Ok(versions);
            }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi server khi lấy danh sách version cho Org {OrgId}", organizationId);
                return StatusCode(500, "Lỗi máy chủ.");
            }
        }

        [HttpGet("versions/{datasetVersionId}/download")]
        public async Task<IActionResult> DownloadSubscribedVersion([FromRoute] Guid datasetVersionId, CancellationToken ct = default)
        {
            var organizationId = GetOrganizationIdFromToken();
            if (organizationId == Guid.Empty) return Unauthorized("Token API Key không hợp lệ.");

            try
            {
                var downloadLink = await _servicesProvider.DataConsumerService
                    .GetSubscribedVersionLinkAsync(organizationId, datasetVersionId, ct);

                return Redirect(downloadLink);
            }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi server khi tải version thuê bao cho Org {OrgId}", organizationId);
                return StatusCode(500, "Lỗi máy chủ khi tạo link download.");
            }
        }

        private Guid GetOrganizationIdFromToken()
        {
            var orgIdClaim = User.FindFirstValue("OrganizationId");
            Guid.TryParse(orgIdClaim, out var organizationId);
            return organizationId;
        }
    }
}
