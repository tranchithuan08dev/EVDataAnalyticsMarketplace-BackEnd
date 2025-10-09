using EV.AdminService.API.DTOs.Result;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IModerationService
    {
        Task<ModerationResult> AnalyzeDatasetAsync(Guid datasetId, string title, string shortDescription, CancellationToken ct = default);
    }
}
