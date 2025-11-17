using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDetailDTO>> GetActiveSubscriptionsAsync(CancellationToken ct = default);
        Task<bool> CancelSubscriptionAsync(Guid subscriptionId, CancellationToken ct = default);
    }
}
