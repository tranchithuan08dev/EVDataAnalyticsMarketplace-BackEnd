using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IDataConsumerService
    {
        Task<IEnumerable<DatasetVersionMetadataDTO>> GetDatasetVersionsAsync(Guid organizationId, Guid datasetId, CancellationToken ct);
        Task<string> GetSubscribedVersionLinkAsync(Guid organizationId, Guid datasetVersionId, CancellationToken ct);
    }
}
