using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;
using Microsoft.AspNetCore.OData.Query;

namespace EV.DataConsumerService.API.Service
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetRepository _repository;
        public DatasetService(IDatasetRepository repository)
        {
            _repository = repository;
        
        }
        [EnableQuery]
        public IQueryable<DatasetSearchResultDto> GetPublicDatasetsForSearch()
        {
            var datasets = _repository.FindAllPublicDatasets();

            return datasets.Select(d => new DatasetSearchResultDto
            {
                DatasetId = d.DatasetId,
                Title = d.Title,
                Category = d.Category,
                Region = d.Region,

                FileFormat = d.DatasetVersions.Select(v => v.FileFormat).FirstOrDefault(),
                MinPricePerDownload = d.DatasetVersions.Min(v => v.PricePerDownload) ?? 0,
                HasSubscriptionOption = d.DatasetVersions.Any(v => v.SubscriptionRequired)
            });

            
        }
    }
}
