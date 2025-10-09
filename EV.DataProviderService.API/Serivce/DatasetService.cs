using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;
using Prometheus;

namespace EV.DataProviderService.API.Service
{
    public class DatasetService : IDatasetService
    {
            private readonly IDatasetRepository _repository;

            public DatasetService(IDatasetRepository repository)
            {
                _repository = repository;
            }
        // 1. Khai báo Metric Counter: Đếm số lần tìm kiếm được thực hiện
        private static readonly Counter DatasetSearchCount = Metrics
            .CreateCounter("marketplace_dataset_searches_total",
                           "Total number of times the public dataset search service was called.");

        // 2. Khai báo Metric Summary: Đo lường thời gian thực thi của Service
        private static readonly Summary DatasetSearchDuration = Metrics
            .CreateSummary("marketplace_dataset_search_duration_seconds",
                           "Duration of the GetPublicDatasetsForSearch service method.",
                           new SummaryConfiguration { MaxAge = TimeSpan.FromMinutes(5), Objectives = new[] { new QuantileEpsilonPair(0.5, 0.05), new QuantileEpsilonPair(0.9, 0.01), new QuantileEpsilonPair(0.99, 0.001) } });

        public async Task<List<DatasetProviderListDto>> GetAllDatasetsAsync(Guid providerId)
        {
            using (DatasetSearchDuration.NewTimer())
            {
              
                DatasetSearchCount.Inc();

                var datasets = await _repository.GetAllDatasetsByProviderIdAsync(providerId);

                return datasets.Select(d => new DatasetProviderListDto
                {
                    DatasetId = d.DatasetId,
                    Title = d.Title,
                    ShortDescription = d.ShortDescription,
                    Category = d.Category,
                    Region = d.Region,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt
                }).ToList();
            } 
        }
    }
  }
    
