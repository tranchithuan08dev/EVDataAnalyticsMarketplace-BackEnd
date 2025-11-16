using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EV.DataConsumerService.API.Data.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public OrganizationRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả các Tổ chức từ bảng Organizations.
        /// </summary>
        public async Task<List<OrganizationDto>> GetAllOrganizationsAsync()
        {
            var organizations = new List<OrganizationDto>();
            var connectionString = _context.Database.GetConnectionString();

            // Lấy tất cả các cột cần thiết từ bảng Organizations
            string sql = @"
                SELECT OrganizationId, Name, OrgType, Description, Country, CreatedAt
                FROM Organizations
                ORDER BY Name ASC";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            organizations.Add(new OrganizationDto
                            {
                                OrganizationId = reader.GetGuid(reader.GetOrdinal("OrganizationId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),

                                // Xử lý các trường có thể là NULL trong DB
                                OrgType = reader.IsDBNull(reader.GetOrdinal("OrgType"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("OrgType")),

                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Description")),

                                Country = reader.IsDBNull(reader.GetOrdinal("Country"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Country")),

                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            });
                        }
                    }
                }
            }
            return organizations;
        }
    }
}
