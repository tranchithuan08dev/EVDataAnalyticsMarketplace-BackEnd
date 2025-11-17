using EV.DataProviderService.API.Models.DTOs;

namespace EV.DataProviderService.API.Service
{
    public interface IAnonymizationService
    {
        Task<AnonymizationResultDto> AnonymizeDatasetAsync(AnonymizationRequestDto request);
    }
}