using EV.AdminService.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<object?> GetUsersByRoleAsync(string role, CancellationToken ct = default);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken ct = default);
        Task<bool> UpdateUserRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken ct = default);
        Task<bool> SetUserActiveAsync(Guid userId, bool isActive, CancellationToken ct = default);
        Task<object?> GetProvidersAsync(CancellationToken ct = default);
        Task<object?> GetConsumersAsync(CancellationToken ct = default);
    }
}
