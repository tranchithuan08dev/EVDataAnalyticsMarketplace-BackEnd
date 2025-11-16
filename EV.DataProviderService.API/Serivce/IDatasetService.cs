using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;

namespace EV.DataProviderService.API.Service
{
    public interface IDatasetService
    {
        Task<List<DatasetProviderListDto>> GetAllDatasetsAsync(Guid providerId);
        Task<ProviderDatasetDetailDto> GetProviderDetailsWithDatasetsAsync(Guid providerId);

        Task<DatasetDetailFullDto> GetDetailDatasetAsync(Guid datasetId);
    }
}