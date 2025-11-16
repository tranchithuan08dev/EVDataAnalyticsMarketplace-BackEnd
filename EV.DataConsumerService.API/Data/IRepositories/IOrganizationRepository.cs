using EV.DataConsumerService.API.Models.DTOs;

namespace EV.DataConsumerService.API.Data.IRepositories
{
    public interface IOrganizationRepository
    {
        Task<List<OrganizationDto>> GetAllOrganizationsAsync();
    }
}
