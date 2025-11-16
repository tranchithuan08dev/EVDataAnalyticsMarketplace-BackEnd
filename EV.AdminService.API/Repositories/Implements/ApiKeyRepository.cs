using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Basics;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Repositories.Implements
{
    public class ApiKeyRepository : CRUDRepository<ApiKey>
    {
        public ApiKeyRepository(EVDataAnalyticsMarketplaceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApiKeyDTO>> GetActiveApiKeysAsync(CancellationToken ct = default)
        {
            var apikeys = await _dbSet.AsNoTracking().Where(k => !k.Revoked).Include(k => k.Organization)
                                    .Select(k => new ApiKeyDTO
                                    {
                                        ApiKeyId = k.ApiKeyId,
                                        OrganizationName = k.Organization.Name,
                                        Description = k.Description,
                                        CreatedAt = k.CreatedAt,
                                        ExpiresAt = k.ExpiresAt,
                                        Revoked = k.Revoked
                                    })
                                    .OrderByDescending(k => k.CreatedAt)
                                    .ToListAsync(ct).ConfigureAwait(false);
            return apikeys;
        }
    }
}
