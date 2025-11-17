using EV.AdminService.API.AI.Data;
using EV.AdminService.API.AI.GeminiDTOs;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System.Net.Http;
using System.Text.Json;

namespace EV.AdminService.API.Services.Implements
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MLContext _mlContext;
        public AnalyticsService(IUnitOfWork unitOfWork, MLContext mlContext, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _mlContext = mlContext;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<AnalysisReportDTO>> GetAITrendReportsAsync(CancellationToken ct = default)
        {
            var analyses = await _unitOfWork.AnalysisRepository.GetPublicOrRestrictedAnalysisAsync(ct);
            return analyses;
        }

        public async Task<CombinedAnalysisReport> GetDemandForecastAsync(int horizon, CancellationToken ct = default)
        {
            var forecastData = await RunForecastingModelAsync(horizon, ct);

            var analysisText = await AnalyzeForecastWithGeminiApiAsync(forecastData, ct);

            return new CombinedAnalysisReport
            {
                QualitativeReport = analysisText,
                ForecastData = forecastData
            };
        }

        private async Task<DemandForecast> RunForecastingModelAsync(int horizon, CancellationToken ct = default)
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

        private async Task<string> AnalyzeForecastWithGeminiApiAsync(DemandForecast forecast, CancellationToken ct = default)
        {
            if (forecast.ForecastedValues == null || forecast.ForecastedValues.Length == 0)
            {
                return "Không đủ dữ liệu để tạo phân tích.";
            }

            try
            {
                string apiKey = _configuration["Gemini:ApiKey"]!;
                string baseUrl = _configuration["Gemini:BaseUrl"]!;
                string model = _configuration["Gemini:Model"]!;
                string apiUrl = $"{baseUrl}/v1beta/models/{model}:generateContent?key={apiKey}";

                var forecastJson = JsonSerializer.Serialize(forecast);

                var prompt = $@"Bạn là một nhà phân tích dữ liệu cấp cao.
                                Dưới đây là kết quả dự báo nhu cầu (JSON):
                                {forecastJson}
                                Nhiệm vụ của bạn: Viết một báo cáo phân tích (dưới 150 từ) bằng tiếng Việt.
                                - Đưa ra lập luận về Xu hướng (Trend).
                                - Đưa ra lập luận về Độ tin cậy (Confidence).
                                - Đưa ra 1 Lời khuyên (Recommendation).";

                var requestBody = new GeminiRequest
                {
                    Contents = new List<GeminiContent> {
                        new GeminiContent {
                            Parts = new List<GeminiPart> {
                                new GeminiPart { Text = prompt }
                            }
                        }
                    }
                };

                var client = _httpClientFactory.CreateClient("Gemini");
                var response = await client.PostAsJsonAsync(apiUrl, requestBody, ct);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(ct);
                    return $"Lỗi gọi API phân tích: {response.ReasonPhrase}";
                }

                var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: ct);
                var analysisText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                return string.IsNullOrEmpty(analysisText) ? "Không nhận được phân tích từ AI." : analysisText;
            }
            catch (Exception)
            {
                return "Hệ thống AI phân tích đang tạm thời gián đoạn.";
            }
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
