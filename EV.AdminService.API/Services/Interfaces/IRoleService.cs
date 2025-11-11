using EV.AdminService.API.DTOs.Requests;
using EV.AdminService.API.Models;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken ct = default);
        Task<Role?> GetRoleByIdAsync(int roleId, CancellationToken ct = default);
        Task<Role> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default);
        Task<bool> UpdateRoleAsync(int roleId, UpdateRoleRequest request, CancellationToken ct = default);
        Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default);
    }
}
