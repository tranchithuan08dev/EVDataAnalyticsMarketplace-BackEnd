namespace EV.AdminService.API.Services.Interfaces
{
    public interface IDatasetService
    {
        Task<object?> GetPendingDatasetsAsync(CancellationToken ct = default);
        Task<bool> SetDatasetStatusAsync(Guid datasetId, string status, CancellationToken ct = default);
    }
}
