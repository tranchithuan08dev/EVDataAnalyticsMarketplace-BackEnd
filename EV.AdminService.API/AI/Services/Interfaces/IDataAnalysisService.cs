using EV.AdminService.API.AI.Data;
using EV.AdminService.API.Models;

namespace EV.AdminService.API.AI.Services.Interfaces
{
    public interface IDataAnalysisService
    {
        Task<List<DataQualityFlag>> DetectSpikesAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct = default);
        Task<List<DataQualityFlag>> DetectChangePointsAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct = default);
        Task<List<DataQualityFlag>> DetectCheatingRulesAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct = default);
        Task<List<DataQualityFlag>> DetectPiiAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct = default);
    }
}
