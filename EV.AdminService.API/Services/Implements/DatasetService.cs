using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class DatasetService : IDatasetService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DatasetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<object?> GetPendingDatasetsAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.DatasetRepository.GetPendingDatasetAsync(ct);
        }

        public async Task<bool> SetDatasetStatusAsync(Guid datasetId, string status, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            status = status.Trim().ToLowerInvariant();
            if (status != "approved" && status != "rejected" && status != "pending")
            {
                return false;
            }

            return await _unitOfWork.DatasetRepository.SetStatusAsync(datasetId, char.ToUpper(status[0]) + status.Substring(1), ct);
        }
    }
}
