using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV.AdminService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ModerationController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;
        public ModerationController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        [HttpGet("pending-datasets")]
        public async Task<IActionResult> GetPendingDatasets(CancellationToken ct)
        {
            var pendingDatasets = await _servicesProvider.AdminModerationService.GetPendingDatasetsAsync(ct).ConfigureAwait(false);
            return Ok(pendingDatasets);
        }

        //Xem chi tiết flag của AI gắn
        [HttpGet("dataset-version/{datasetVersionId}/moderation-detail")]
        public async Task<IActionResult> GetModerationDetail([FromRoute] Guid datasetVersionId, CancellationToken ct)
        {
            var detail = await _servicesProvider.AdminModerationService.GetModerationDetailAsync(datasetVersionId, ct).ConfigureAwait(false);
            if (detail == null)
            {
                return NotFound("Versions not found");
            }
            return Ok(detail);
        }

        [HttpPost("approve/{datasetId}")]
        public async Task<IActionResult> ApproveDataset([FromRoute] Guid datasetId, CancellationToken ct)
        {
            var success = await _servicesProvider.AdminModerationService.ApproveDatasetAsync(datasetId, ct).ConfigureAwait(false);
            if (!success)
            {
                return BadRequest("Dataset not found or not in pending status");
            }
            return Ok("Dataset approved successfully");
        }

        [HttpPost("reject/{datasetId}")]
        public async Task<IActionResult> RejectDataset([FromRoute] Guid datasetId, CancellationToken ct)
        {
            var success = await _servicesProvider.AdminModerationService.RejectDatasetAsync(datasetId, ct).ConfigureAwait(false);
            if (!success)
            {
                return BadRequest("Dataset not found or not in pending status");
            }
            return Ok("Dataset rejected successfully");
        }
    }
}
