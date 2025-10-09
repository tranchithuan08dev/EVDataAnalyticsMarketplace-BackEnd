using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class ConsumerRepository : CRUDRepository<Consumer>
    {
        public ConsumerRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<object?> GetConsumer(CancellationToken ct = default)
        {
            return await _context.Consumers
                    .AsNoTracking()
                    .Include(c => c.Organization)
                    .Select(c => new
                    {
                        c.ConsumerId,
                        Organization = new { c.Organization.OrganizationId, c.Organization.Name, c.Organization.OrgType }
                    })
                    .ToListAsync(ct);
        }
    }
}
