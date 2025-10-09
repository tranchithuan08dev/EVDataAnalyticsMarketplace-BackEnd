using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Models.DTOs;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class UserRepository : CRUDRepository<User>
    {
        public UserRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<UserDto>> GetUserDtosAsync(CancellationToken ct = default)
        {
            var users = await _context.Users.AsNoTracking()
                                .Include(u => u.Roles)
                                .Select(u => new UserDto
                                {
                                    UserId = u.UserId,
                                    Email = u.Email,
                                    DisplayName = u.DisplayName,
                                    IsActive = u.IsActive,
                                    Roles = u.Roles.Select(r => r.Name).ToList()
                                })
                                .ToListAsync(ct)
                                .ConfigureAwait(false);
            return users;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId, ct).ConfigureAwait(false);
        }

        public async Task<object?> GetUserDTOsByRoleId(Role roleEntity, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.RoleId == roleEntity.RoleId))
                .Select(u => new Models.DTOs.UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    IsActive = u.IsActive,
                    Roles = u.Roles.Select(r => r.Name).ToList()
                })
                .ToListAsync(ct);
        }
    }
}
