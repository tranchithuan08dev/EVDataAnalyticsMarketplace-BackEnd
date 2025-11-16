using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Repositories;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Service
{
    public class RevenueService : IRevenueService
    {
        private readonly IRevenueRepository _revenueRepository;

        public RevenueService(IRevenueRepository revenueRepository)
        {
            _revenueRepository = revenueRepository;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId)
        {
            return await _revenueRepository.GetRevenueReportAsync(providerId);
        }
    }
}