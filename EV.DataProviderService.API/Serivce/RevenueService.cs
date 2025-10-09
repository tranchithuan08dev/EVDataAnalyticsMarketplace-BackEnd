using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Repositories;
using Prometheus;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Service
{
    public class RevenueService : IRevenueService
    {
        private readonly IRevenueRepository _revenueRepository;

        // 1. Khai báo Metric Counter: Đếm số lần tìm kiếm được thực hiện
        private static readonly Counter DatasetSearchCount = Metrics
            .CreateCounter("marketplace_dataset_searches_total",
                           "Total number of times the public dataset search service was called.");

        // 2. Khai báo Metric Summary: Đo lường thời gian thực thi của Service
        private static readonly Summary DatasetSearchDuration = Metrics
            .CreateSummary("marketplace_dataset_search_duration_seconds",
                           "Duration of the GetPublicDatasetsForSearch service method.",
                           new SummaryConfiguration { MaxAge = TimeSpan.FromMinutes(5), Objectives = new[] { new QuantileEpsilonPair(0.5, 0.05), new QuantileEpsilonPair(0.9, 0.01), new QuantileEpsilonPair(0.99, 0.001) } });

        public RevenueService(IRevenueRepository revenueRepository)
        {
            _revenueRepository = revenueRepository;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId, DateTime? startDate, DateTime? endDate)
        {

            using (DatasetSearchDuration.NewTimer())
            {
                // Tăng bộ đếm mỗi khi hàm được gọi
                DatasetSearchCount.Inc();
                return await _revenueRepository.GetRevenueReportAsync(providerId, startDate, endDate);

            }

        }
    }
}