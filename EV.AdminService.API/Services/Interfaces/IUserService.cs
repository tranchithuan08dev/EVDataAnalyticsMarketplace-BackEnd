using EV.AdminService.API.DTOs.DataModels;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDetailDTO>> GetUsersAsync(CancellationToken ct = default);
        Task<bool> SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default);
        Task<bool> UpdateUserRolesAsync(Guid userId, List<int> roleIds, CancellationToken ct = default);
        Task<bool> VerifyProviderAsync(Guid organizationId, CancellationToken ct = default);
    }
}
