using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;
using EV.DataProviderService.API.Repositories;

namespace EV.DataProviderService.API.Service
{
    public class AnonymizationService : IAnonymizationService
    {
        private readonly IAnonymizationRepository _repo;

        public AnonymizationService(IAnonymizationRepository repo)
        {
            _repo = repo;
        }

        public async Task<AnonymizationResultDto> AnonymizeDatasetAsync(AnonymizationRequestDto request)
        {
            try
            {
                var success = await _repo.AnonymizeDatasetFilesAsync(
                    request.DatasetVersionId, request.FieldsToAnonymize, request.Method);

                if (!success)
                    return new AnonymizationResultDto { Status = "Failed", Message = "No files found" };

                var log = new AnonymizationLog
                {
                    AnonymizationId = Guid.NewGuid(),
                    DatasetVersionId = request.DatasetVersionId,
                    PerformedBy = request.PerformedBy,
                    Method = request.Method,
                    Details = $"Fields: {string.Join(", ", request.FieldsToAnonymize)}",
                    PerformedAt = DateTime.UtcNow
                };

                await _repo.LogAnonymizationAsync(log);

                return new AnonymizationResultDto
                {
                    AnonymizationId = log.AnonymizationId,
                    DatasetVersionId = request.DatasetVersionId,
                    Method = request.Method,
                    PerformedAt = log.PerformedAt,
                    Status = "Success",
                    Message = "Anonymized and logged."
                };
            }
            catch (Exception ex)
            {
                return new AnonymizationResultDto
                {
                    DatasetVersionId = request.DatasetVersionId,
                    Status = "Error",
                    Message = ex.Message
                };
            }
        }
    }
}