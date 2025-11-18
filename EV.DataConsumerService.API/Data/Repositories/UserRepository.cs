using EV.DataConsumerService.API.Data.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EV.DataConsumerService.API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public UserRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        public async Task<Guid?> GetOrganizationIdByUserIdAsync(Guid userId)
        {
            var connectionString = _context.Database.GetConnectionString();
            string sql = "SELECT OrganizationId FROM Users WHERE UserId = @UserId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        return (Guid)result;
                    }
                    return null;
                }
            }
        }

        public async Task<Guid?> GetProviderIdByOrganizationIdAsync(Guid organizationId)
        {
            var connectionString = _context.Database.GetConnectionString();
            string sql = "SELECT ProviderId FROM Providers WHERE OrganizationId = @OrganizationId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@OrganizationId", organizationId);
                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        return (Guid)result;
                    }
                    return null;
                }
            }
        }
    }
}
