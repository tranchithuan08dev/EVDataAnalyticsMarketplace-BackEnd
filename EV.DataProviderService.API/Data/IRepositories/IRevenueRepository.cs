using EV.DataProviderService.API.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace EV.DataProviderService.API.Repositories
{
    public interface IRevenueRepository
    {
        Task<RevenueReportDto> GetRevenueReportAsync(Guid providerId);
    }
}