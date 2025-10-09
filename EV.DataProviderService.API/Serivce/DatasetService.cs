using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;

namespace EV.DataProviderService.API.Service
{
    public class DatasetService : IDatasetService
    {
            private readonly IDatasetRepository _repository;

            public DatasetService(IDatasetRepository repository)
            {
                _repository = repository;
            }

        public async Task<List<DatasetProviderListDto>> GetAllDatasetsAsync(Guid providerId)
        {
            var datasets = await _repository.GetAllDatasetsByProviderIdAsync(providerId);

            // Mapping từ Entity (Dataset) sang DTO (DatasetProviderListDto)
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
