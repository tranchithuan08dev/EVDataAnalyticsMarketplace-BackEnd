using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using EV.AdminService.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EV.AdminService.API.Repositories.Implements
{
    public class RoleRepository : CRUDRepository<Role>
    {
        public RoleRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken ct)
        {
            return await _context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase), ct)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Role>> GetByNamesAsync(IEnumerable<string> roleNames, CancellationToken ct)
        {
            return await _context.Roles.AsNoTracking()
                .Where(r => roleNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
    }
}
