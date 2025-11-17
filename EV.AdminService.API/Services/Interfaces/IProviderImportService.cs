namespace EV.AdminService.API.Services.Interfaces
{
    public interface IProviderImportService
    {
        Task<Guid> ImportNewDatasetAsync(IFormFile metadataFile, IFormFile dataFile, Guid providerId, CancellationToken ct);
    }
}
