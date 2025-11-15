using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;


namespace EV.DataProviderService.API.Data.IRepositories
{
    public interface IDatasetRepository
    {
        Task<List<Dataset>> GetAllDatasetsByProviderIdAsync(Guid providerId);

        Task<ProviderDetailDto> GetProviderDetailsAsync(Guid providerId);

        Task<IEnumerable<ProviderDatasetSummaryDto>> GetDatasetsByProviderIdAsync(Guid providerId);

        Task<DatasetDetailFullDto> GetDatasetHeaderDetailAsync(Guid datasetId);

        
        Task<IEnumerable<DatasetVersionDetailDto>> GetDatasetVersionsAsync(Guid datasetId);

     
        Task<IEnumerable<DataFileDto>> GetDataFilesByVersionIdAsync(Guid datasetVersionId);

    }
}