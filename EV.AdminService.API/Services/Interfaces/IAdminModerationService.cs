using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IAdminModerationService
    {
        Task<IEnumerable<PendingModerationDTO>> GetPendingDatasetsAsync(CancellationToken ct = default);
        Task<ModerationDetailDTO?> GetModerationDetailAsync(Guid datasetVersionId, CancellationToken ct = default);
        Task<bool> ApproveDatasetAsync(Guid datasetId, CancellationToken ct = default);
        Task<bool> RejectDatasetAsync(Guid datasetId, CancellationToken ct = default);
    }
}
