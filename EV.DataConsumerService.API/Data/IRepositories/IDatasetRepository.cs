using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Models.Entities;

namespace EV.DataConsumerService.API.Data.IRepositories
{
    public interface IDatasetRepository
    {
      
        IQueryable<Dataset> FindAllPublicDatasets();

        IQueryable<Dataset> GetFullPublicDatasetsQuery();

        Task<IEnumerable<DatasetSearchDetailDto>> SearchDatasetsAsync(DatasetSearchRequestDto searchRequest);
        Task ExecutePurchaseAsync(PurchaseRequestDto purchaseRequest);

        Task<SubscriptionResponseDto> ExecuteSubscriptionAndApiKeyCreationAsync(
        SubscriptionRequestDto request,
        byte[] apiKeyHash,
        DateTime expiresAt,
        string plainApiKey);
    }
}
