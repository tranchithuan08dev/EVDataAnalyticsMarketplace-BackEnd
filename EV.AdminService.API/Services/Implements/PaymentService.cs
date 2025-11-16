using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> DistributeRevenueAsync(Guid paymentId, CancellationToken ct = default)
        {
            return await _unitOfWork.PaymentRepository.DistributeRevenueAsync(paymentId, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.PaymentRepository.GetPendingPaymentsAsync(ct).ConfigureAwait(false);
        }
    }
}
