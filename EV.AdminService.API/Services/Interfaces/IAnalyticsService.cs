using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<IEnumerable<PopularDatasetDTO>> GetPopularDatasetsAsync(CancellationToken ct = default);
        Task<IEnumerable<AnalysisReportDTO>> GetAITrendReportsAsync(CancellationToken ct = default);
    }
}
