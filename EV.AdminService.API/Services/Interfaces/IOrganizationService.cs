using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDetailDTO>> GetOrganizationsAsync(CancellationToken ct = default);
    }
}
