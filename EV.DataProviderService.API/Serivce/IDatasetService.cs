using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;

namespace EV.DataProviderService.API.Service
{
    public interface IDatasetService
    {
        Task<List<DatasetProviderListDto>> GetAllDatasetsAsync(Guid providerId);
    }
}