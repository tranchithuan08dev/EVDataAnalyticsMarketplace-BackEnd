using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Basic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EV.AdminService.API.Repositories.Implements
{
    public class ApiKeyRepository : CRUDRepository<ApiKey>
    {
        public ApiKeyRepository(EVDataAnalyticsMarketplaceContext context) : base(context)
        {
        }

        public async Task<ApiKey?> GetByActiveKeyHashAsync(string key, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            byte[] providedHash;
            using (var sha = SHA256.Create())
            {
                providedHash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            }

            var now = DateTime.UtcNow;

            var candidates = await _context.ApiKeys.AsNoTracking()
                .Where(k => !k.Revoked && (k.ExpiresAt == null || k.ExpiresAt > now))
                .Include(k => k.Organization)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            foreach (var candidate in candidates)
            {
                if (candidate.KeyHash == null || candidate.KeyHash.Length == 0)
                {
                    continue;
                }

                if (CryptographicOperations.FixedTimeEquals(candidate.KeyHash, providedHash))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
