using EV.AdminService.API.AI.Data;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace EV.AdminService.API.Services.Implements
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MLContext _mlContext;
        public AnalyticsService(IUnitOfWork unitOfWork, MLContext mlContext)
        {
            _unitOfWork = unitOfWork;
            _mlContext = mlContext;
        }

        public async Task<IEnumerable<AnalysisReportDTO>> GetAITrendReportsAsync(CancellationToken ct = default)
        {
            var analyses = await _unitOfWork.AnalysisRepository.GetPublicOrRestrictedAnalysisAsync(ct);
            return analyses;
        }

        public async Task<DemandForecast> GetDemandForecastAsync(int horizon, CancellationToken ct = default)
        {
            var purchaseHistory = await _unitOfWork.PurchaseRepository
                .Query()
                .GroupBy(p => p.PurchasedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);
            var subscriptionHistory = await _unitOfWork.SubscriptionRepository
                .Query()
                .GroupBy(p => p.StartedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var dailyInterest = purchaseHistory
                .Select(p => new TrendDataPoint { Date = p.Date, Value = p.Count })
                .Concat(subscriptionHistory.Select(s => new TrendDataPoint { Date = s.Date, Value = s.Count }))
                .GroupBy(x => x.Date)
                .Select(g => new TrendDataPoint { Date = g.Key, Value = g.Sum(x => x.Value) })
                .OrderBy(t => t.Date)
                .ToList();

            if (dailyInterest.Count < 14)
            {
                return new DemandForecast { ForecastedValues = new float[0] };
            }

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(DemandForecast.ForecastedValues),
                inputColumnName: nameof(TrendDataPoint.Value),
                windowSize: 7,
                seriesLength: dailyInterest.Count,
                trainSize: dailyInterest.Count,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(DemandForecast.LowerBoundValues),
                confidenceUpperBoundColumn: nameof(DemandForecast.UpperBoundValues));

            var dataView = _mlContext.Data.LoadFromEnumerable(dailyInterest);
            var model = pipeline.Fit(dataView);

            var forecastingEngine = model.CreateTimeSeriesEngine<TrendDataPoint, DemandForecast>(_mlContext);
            var forecast = forecastingEngine.Predict();

            forecast.ForecastedValues = forecast.ForecastedValues!.Select(v => Math.Max(0, v)).ToArray();
            return forecast;
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
