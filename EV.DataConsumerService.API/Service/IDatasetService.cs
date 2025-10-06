using EV.DataConsumerService.API.Models.DTOs;

namespace EV.DataConsumerService.API.Service
{
    public interface IDatasetService
    {
        IQueryable<DatasetSearchResultDto> GetPublicDatasetsForSearch();
    }
}
