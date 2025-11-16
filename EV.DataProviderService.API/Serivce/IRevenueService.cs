using EV.DataProviderService.API.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Service
{
    public interface IRevenueService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId);
    }
}