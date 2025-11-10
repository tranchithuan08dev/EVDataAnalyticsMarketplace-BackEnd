using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Models.Entities;

namespace EV.DataConsumerService.API.Data.IRepositories
{
    public interface IDatasetRepository
    {
      
        IQueryable<Dataset> FindAllPublicDatasets();

        IQueryable<Dataset> GetFullPublicDatasetsQuery();

        Task<IEnumerable<DatasetSearchDetailDto>> SearchDatasetsAsync(DatasetSearchRequestDto searchRequest);
    }
}
