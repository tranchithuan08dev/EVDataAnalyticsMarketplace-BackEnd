using EV.DataConsumerService.API.Data.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EV.DataConsumerService.API.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public AuthRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        // Giả định: 2: provider, 3: consumer dựa trên EVDatabase.sql
        private const int CONSUMER_ROLE_ID = 3;
        private const int PROVIDER_ROLE_ID = 2;

        public async Task<bool> UserExistsAsync(string email)
        {
            var connectionString = _context.Database.GetConnectionString();
            string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return (int)await command.ExecuteScalarAsync() > 0;
                }
            }
        }

        public async Task<Guid> RegisterUserAsync(string email, byte[] passwordHash, string displayName, Guid? organizationId, int roleId)
        {
            var connectionString = _context.Database.GetConnectionString();
            Guid newUserId = Guid.NewGuid();

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert vào bảng Users
                        string userSql = @"
                            INSERT INTO Users (UserId, Email, PasswordHash, DisplayName, OrganizationId)
                            VALUES (@UserId, @Email, @PasswordHash, @DisplayName, @OrganizationId)";

                        await using (var userCommand = new SqlCommand(userSql, connection, transaction))
                        {
                            userCommand.Parameters.AddWithValue("@UserId", newUserId);
                            userCommand.Parameters.AddWithValue("@Email", email);
                            userCommand.Parameters.AddWithValue("@PasswordHash", passwordHash);
                            userCommand.Parameters.AddWithValue("@DisplayName", displayName);
                            userCommand.Parameters.AddWithValue("@OrganizationId", (object)organizationId ?? DBNull.Value);

                            await userCommand.ExecuteNonQueryAsync();
                        }

                        // 2. Insert vào bảng UserRoles
                        string roleSql = @"
                            INSERT INTO UserRoles (UserId, RoleId)
                            VALUES (@UserId, @RoleId)";

                        await using (var roleCommand = new SqlCommand(roleSql, connection, transaction))
                        {
                            roleCommand.Parameters.AddWithValue("@UserId", newUserId);
                            roleCommand.Parameters.AddWithValue("@RoleId", roleId);
                            await roleCommand.ExecuteNonQueryAsync();
                        }

                        // 3. Nếu có OrganizationId, insert vào bảng Consumers
                        if (organizationId.HasValue)
                        {
                            // ConsumerId là khóa chính nhưng không phải IDENTITY, nên dùng lại OrganizationId 
                            // *Lưu ý: DDL của bạn thiết lập ConsumerId là PK và OrganizationId là UNIQUE/FK. 
                            // Tốt nhất là ConsumerId = OrganizationId hoặc NEWID(). 
                            // Tôi sẽ dùng lại OrganizationId theo thiết kế phổ biến cho các bảng Provider/Consumer.
                            string consumerSql = @"
                                INSERT INTO Consumers (ConsumerId, OrganizationId, ContactEmail)
                                VALUES (@ConsumerId, @OrganizationId, @ContactEmail)";

                            await using (var consumerCommand = new SqlCommand(consumerSql, connection, transaction))
                            {
                                consumerCommand.Parameters.AddWithValue("@ConsumerId", organizationId.Value);
                                consumerCommand.Parameters.AddWithValue("@OrganizationId", organizationId.Value);
                                consumerCommand.Parameters.AddWithValue("@ContactEmail", email); // Dùng email làm ContactEmail

                                await consumerCommand.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return newUserId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<(Guid UserId, byte[] PasswordHash, int RoleId)?> GetUserCredentialsAsync(string email)
        {
            var connectionString = _context.Database.GetConnectionString();

            // Lấy Hash, UserId và RoleId cao nhất (nếu có nhiều Role)
            string sql = @"
                SELECT u.UserId, u.PasswordHash, MAX(ur.RoleId) AS RoleId
                FROM Users u
                JOIN UserRoles ur ON u.UserId = ur.UserId
                WHERE u.Email = @Email AND u.IsActive = 1
                GROUP BY u.UserId, u.PasswordHash";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return (
                                reader.GetGuid(reader.GetOrdinal("UserId")),
                                (byte[])reader["PasswordHash"],
                                reader.GetInt32(reader.GetOrdinal("RoleId"))
                            );
                        }
                    }
                }
            }
            return null;
        }

        public async Task<string> GetRoleNameByIdAsync(int roleId)
        {
            var connectionString = _context.Database.GetConnectionString();
            string sql = "SELECT Name FROM Roles WHERE RoleId = @RoleId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@RoleId", roleId);
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }
    }
}
