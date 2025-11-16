using EV.DataConsumerService.API.Models.DTOs;

namespace EV.DataConsumerService.API.Service
{
    public interface IOrganizationService
    {
        Task<List<OrganizationDto>> GetAllOrganizationsAsync();
    }
}
