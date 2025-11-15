using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Models.DTOs;
using EV.DataProviderService.API.Models.Entites;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EV.DataProviderService.API.Data.Repositories
{
    public class DatasetRepository : IDatasetRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public DatasetRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }


        public async Task<List<Dataset>> GetAllDatasetsByProviderIdAsync(Guid providerId)
        {
            // Truy vấn tất cả Datasets có ProviderId khớp
            return await _context.Datasets
                 .ToListAsync();
        }



        // --- Phương thức mới: Lấy chi tiết Provider ---
        public async Task<ProviderDetailDto> GetProviderDetailsAsync(Guid providerId)
        {
            var connectionString = _context.Database.GetConnectionString();
            ProviderDetailDto providerDetails = null;

            string sql = @"
                SELECT 
                    p.ProviderId, p.ContactEmail, p.Verified, 
                    o.OrganizationId, o.Name AS OrganizationName, o.OrgType, o.Country
                FROM Providers p
                JOIN Organizations o ON o.OrganizationId = p.OrganizationId
                WHERE p.ProviderId = @ProviderId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProviderId", providerId);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            providerDetails = new ProviderDetailDto
                            {
                                ProviderId = reader.GetGuid(reader.GetOrdinal("ProviderId")),
                                ContactEmail = reader.IsDBNull(reader.GetOrdinal("ContactEmail")) ? null : reader.GetString(reader.GetOrdinal("ContactEmail")),
                                IsVerified = reader.GetBoolean(reader.GetOrdinal("Verified")),
                                OrganizationId = reader.GetGuid(reader.GetOrdinal("OrganizationId")),
                                OrganizationName = reader.GetString(reader.GetOrdinal("OrganizationName")),
                                OrgType = reader.IsDBNull(reader.GetOrdinal("OrgType")) ? null : reader.GetString(reader.GetOrdinal("OrgType")),
                                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country"))
                            };
                        }
                    }
                }
            }
            return providerDetails;
        }

        // --- Phương thức mới: Lấy Datasets theo Provider ID ---
        public async Task<IEnumerable<ProviderDatasetSummaryDto>> GetDatasetsByProviderIdAsync(Guid providerId)
        {
            var datasets = new List<ProviderDatasetSummaryDto>();
            var connectionString = _context.Database.GetConnectionString();

            // 1. CẬP NHẬT TRUY VẤN SQL để lấy tất cả các trường
            string sql = @"
        SELECT 
            DatasetId, ProviderId, Title, ShortDescription, LongDescription, 
            Category, DataTypes, Region, VehicleTypes, BatteryTypes, 
            LicenseType, Visibility, Status, CreatedAt, UpdatedAt
        FROM Datasets
        WHERE ProviderId = @ProviderId
        ORDER BY CreatedAt DESC";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProviderId", providerId);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            datasets.Add(new ProviderDatasetSummaryDto
                            {
                                DatasetId = reader.GetGuid(reader.GetOrdinal("DatasetId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),

                                // 2. ÁNH XẠ CÁC TRƯỜNG MỚI
                                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                                DataTypes = reader.IsDBNull(reader.GetOrdinal("DataTypes")) ? null : reader.GetString(reader.GetOrdinal("DataTypes")),
                                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader.GetString(reader.GetOrdinal("Region")),
                                BatteryTypes = reader.IsDBNull(reader.GetOrdinal("BatteryTypes")) ? null : reader.GetString(reader.GetOrdinal("BatteryTypes")),
                                LicenseType = reader.GetString(reader.GetOrdinal("LicenseType")), // SQL có Default, giả định luôn có giá trị

                                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                                VehicleTypes = reader.IsDBNull(reader.GetOrdinal("VehicleTypes")) ? null : reader.GetString(reader.GetOrdinal("VehicleTypes")),
                                Visibility = reader.GetString(reader.GetOrdinal("Visibility")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                // Ánh xạ UpdatedAt (có thể null)
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            });
                        }
                    }
                }
            }
            return datasets;
        }
    }
    }
