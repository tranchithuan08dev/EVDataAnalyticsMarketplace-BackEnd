using EV.AdminService.API.DTOs.Requests;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IProviderImportService
    {
        Task<Guid> ImportNewDatasetAsync(IFormFile metadataFile, IFormFile dataFile, Guid providerId, CancellationToken ct);
        Task<Guid> CreateDatasetAsync(CreateDatasetRequest request, Guid providerId, CancellationToken ct);
    }
}
