using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class AdminModerationService : IAdminModerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminModerationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ApproveDatasetAsync(Guid datasetId, CancellationToken ct = default)
        {
            var dataset = await _unitOfWork.DatasetRepository.GetByIdAsync(ct, datasetId).ConfigureAwait(false);
            if (dataset == null || dataset.Status != "pending")
            {
                return false;
            }
            
            dataset.Status = "approved";
            dataset.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.DatasetRepository.UpdateAsync(dataset, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }

        public async Task<ModerationDetailDTO?> GetModerationDetailAsync(Guid datasetVersionId, CancellationToken ct = default)
        {
            var version = await _unitOfWork.DatasetVersionRepository.GetDatasetVersionAsync(datasetVersionId, ct).ConfigureAwait(false);
            if (version == null)
            {
                return null;
            }

            return new ModerationDetailDTO
            {
                DatasetId = version.DatasetId,
                DatasetVersionId = version.DatasetVersionId,
                Title = version.Dataset.Title,
                VersionLabel = version.VersionLabel,
                ProviderName = version.Dataset.Provider.Organization.Name,
                SampleFileUri = version.SampleUri,
                Flags = version.DataQualityFlags.Select(f => new DataQualityFlagDTO
                {
                    FlagType = f.FlagType,
                    Message = f.Message,
                    ProcessedBy = f.ProcessedBy,
                    CreatedAt = f.CreatedAt
                }).ToList()
            };
        }

        public async Task<IEnumerable<PendingModerationDTO>> GetPendingDatasetsAsync(CancellationToken ct = default)
        {
            var datasets = await _unitOfWork.DatasetRepository.GetPendingDatasetsAsync(ct).ConfigureAwait(false);

            var result = new List<PendingModerationDTO>();

            foreach (var dataset in datasets)
            {
                var latestVersion = dataset.DatasetVersions
                    .OrderByDescending(dv => dv.CreatedAt)
                    .FirstOrDefault();

                if (latestVersion == null)
                {
                    continue;
                }

                var flags = latestVersion.DataQualityFlags;
                result.Add(new PendingModerationDTO
                {
                    DatasetId = dataset.DatasetId,
                    DatasetVersionId = latestVersion.DatasetVersionId,
                    ProviderName = dataset.Provider.Organization.Name,
                    SubmittedAt = dataset.CreatedAt,
                    RedFlags = flags.Count(f => f.FlagType == "red"),
                    YellowFlags = flags.Count(f => f.FlagType == "yellow"),
                    GreenFlags = flags.Count(f => f.FlagType == "green")
                });
            }

            return result;
        }

        public async Task<bool> RejectDatasetAsync(Guid datasetId, CancellationToken ct = default)
        {
            var dataset = await _unitOfWork.DatasetRepository.GetByIdAsync(ct, datasetId).ConfigureAwait(false);
            if (dataset == null || dataset.Status != "pending")
            {
                return false;
            }

            dataset.Status = "rejected";
            dataset.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.DatasetRepository.UpdateAsync(dataset, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }
    }
}
