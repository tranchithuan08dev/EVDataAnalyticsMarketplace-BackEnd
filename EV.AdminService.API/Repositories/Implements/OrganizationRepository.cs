using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class OrganizationRepository : CRUDRepository<Organization>
    {
        public OrganizationRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrganizationDetailDTO>> GetOrganizationsAsync(CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking().Include(o => o.Consumer).Include(o => o.Provider)
                    .Select(o => new OrganizationDetailDTO
                    {
                        OrganizationId = o.OrganizationId,
                        Name = o.Name,
                        OrgType = o.OrgType,
                        Country = o.Country,
                        IsConsumer = o.Consumer != null,
                        IsProvider = o.Provider != null,
                        IsVerified = (o.Provider != null && o.Provider.Verified)
                    })
                    .ToListAsync(ct).ConfigureAwait(false);
        }
    }
}
