using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class RoleRepository : CRUDRepository<Role>
    {
        public RoleRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Role>> GetRolesAsync(List<int> roleIds, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking().Where(r => roleIds.Contains(r.RoleId)).ToListAsync(ct).ConfigureAwait(false);
        }
    }
}
