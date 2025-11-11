using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubscriptionDetailDTO>> GetActiveSubscriptionsAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.SubscriptionRepository.GetActiveSubscriptionsAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> CancelSubscriptionAsync(Guid subscriptionId, CancellationToken ct = default)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(ct, subscriptionId).ConfigureAwait(false);
            if (subscription == null || !subscription.Active)
            {
                return false;
            }

            subscription.Active = false;
            subscription.ExpiresAt = DateTime.UtcNow;

            await _unitOfWork.SubscriptionRepository.UpdateAsync(subscription, ct).ConfigureAwait(false);
            return true;
        }
    }
}
