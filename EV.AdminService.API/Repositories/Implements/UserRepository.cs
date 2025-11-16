using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class UserRepository : CRUDRepository<User>
    {
        public UserRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserDetailDTO>> GetUsersAsync(CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                    .Include(u => u.Roles)
                    .Include(u => u.Organization)
                    .Select(u => new UserDetailDTO
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        DisplayName = u.DisplayName,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        OrganizationId = u.OrganizationId,
                        Roles = u.Roles.Select(r => r.Name).ToList()
                    })
                    .ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<User?> GetUserWithRoleAsync(Guid userId, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId, ct)
                .ConfigureAwait(false);
        }

        public async Task AddRolesAsync(User user, IEnumerable<Role> roles, CancellationToken ct = default)
        {
            foreach (var role in roles)
            {
                if (!user.Roles.Any(r => r.RoleId == role.RoleId))
                {
                    user.Roles.Add(role);
                }
            }
            _dbSet.Update(user);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }
}
