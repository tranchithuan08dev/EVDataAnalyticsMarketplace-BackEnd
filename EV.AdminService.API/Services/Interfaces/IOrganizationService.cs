using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization?> GetOrganizationByIdAsync(Guid organizationId, CancellationToken ct = default);
        Task<IEnumerable<OrganizationDetailDTO>> GetOrganizationsAsync(CancellationToken ct = default);
    }
}
