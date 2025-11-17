using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
        Task<IEnumerable<UserDetailDTO>> GetUsersAsync(CancellationToken ct = default);
        Task<bool> SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default);
        Task<bool> UpdateUserRolesAsync(Guid userId, List<int> roleIds, CancellationToken ct = default);
        Task<bool> VerifyProviderAsync(Guid organizationId, CancellationToken ct = default);
    }
}
