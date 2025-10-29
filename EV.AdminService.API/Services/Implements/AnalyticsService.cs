using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AnalysisReportDTO>> GetAITrendReportsAsync(CancellationToken ct = default)
        {
            var analyses = await _unitOfWork.AnalysisRepository.GetPublicOrRestrictedAnalysisAsync(ct);
            return analyses;
        }

        public async Task<IEnumerable<PopularDatasetDTO>> GetPopularDatasetsAsync(CancellationToken ct = default)
        {
            var purchaseCounts = await _unitOfWork.PurchaseRepository.GetPurchaseCountsAsync(ct);
            var subCounts = await _unitOfWork.SubscriptionRepository.GetSubsciptionCountsAsync(ct);

            var allDatasetIds = purchaseCounts.Keys.Union(subCounts.Keys);

            var result = allDatasetIds.Select(id =>
            {
                var purchaseInfo = purchaseCounts.GetValueOrDefault(id);
                var subInfo = subCounts.GetValueOrDefault(id, 0);

                var purchaseCount = purchaseInfo.PurchaseCount;
                var totalInterest = (purchaseCount * 1) + (subInfo * 3);

                return new PopularDatasetDTO
                {
                    DatasetId = id,
                    Title = purchaseInfo.Title,
                    PurchaseCount = purchaseCount,
                    SubscriptionCount = subInfo,
                    TotalInterest = totalInterest
                };
            })
            .OrderByDescending(x => x.TotalInterest)
            .Take(10)
            .ToList();
            return result;
        }
    }
}
