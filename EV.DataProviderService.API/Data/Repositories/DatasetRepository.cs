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

        // --- Phương thức 1: Lấy thông tin Header Dataset ---
        public async Task<DatasetDetailFullDto> GetDatasetHeaderDetailAsync(Guid datasetId)
        {
            var connectionString = _context.Database.GetConnectionString();
            DatasetDetailFullDto detail = null;

            string sql = @"
SELECT 
    d.*, 
    p.ProviderId, p.Verified, 
    o.Name AS OrganizationName, o.Country AS OrganizationCountry -- Dùng alias OrganizationName
FROM Datasets d
JOIN Providers p ON p.ProviderId = d.ProviderId
JOIN Organizations o ON o.OrganizationId = p.OrganizationId
WHERE d.DatasetId = @DatasetId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DatasetId", datasetId);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            detail = new DatasetDetailFullDto
                            {
                                DatasetId = reader.GetGuid(reader.GetOrdinal("DatasetId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                              
                                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                                DataTypes = reader.IsDBNull(reader.GetOrdinal("DataTypes")) ? null : reader.GetString(reader.GetOrdinal("DataTypes")),
                                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader.GetString(reader.GetOrdinal("Region")),
                                BatteryTypes = reader.IsDBNull(reader.GetOrdinal("BatteryTypes")) ? null : reader.GetString(reader.GetOrdinal("BatteryTypes")),
                                LicenseType = reader.GetString(reader.GetOrdinal("LicenseType")),
                                Visibility = reader.GetString(reader.GetOrdinal("Visibility")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                // Thông tin Provider/Org
                                ProviderId = reader.GetGuid(reader.GetOrdinal("ProviderId")),
                                OrganizationName = reader.GetString(reader.GetOrdinal("OrganizationName")),
                                OrganizationCountry = reader.IsDBNull(reader.GetOrdinal("OrganizationCountry")) ? null : reader.GetString(reader.GetOrdinal("OrganizationCountry")),
                                IsProviderVerified = reader.GetBoolean(reader.GetOrdinal("Verified")),

                                // SỬA LỖI: Sử dụng tên alias "OrganizationName"
                                ProviderName = reader.GetString(reader.GetOrdinal("OrganizationName"))
                            };
                        }
                    }
                }
            }
            return detail;
        }
        // --- Phương thức 2: Lấy các phiên bản Dataset ---
        public async Task<IEnumerable<DatasetVersionDetailDto>> GetDatasetVersionsAsync(Guid datasetId)
        {
            var versions = new List<DatasetVersionDetailDto>();
            var connectionString = _context.Database.GetConnectionString();

            // SỬA LỖI TRUY VẤN: Lấy đầy đủ các trường và sử dụng ALIAS cho SubscriptionRequired
            string sql = @"
        SELECT 
            DatasetVersionId, DatasetId, VersionLabel, CreatedAt, FileFormat, 
            FilesizeBytes, StorageUri, IsAnalyzed, AnalysisReportUri, SampleUri, 
            PricePerDownload, PricePerGB, 
            SubscriptionRequired AS HasSubscriptionOption, -- Đổi tên cột cho phù hợp với DTO
            AccessPolicyId, LicenseText
        FROM DatasetVersions
        WHERE DatasetId = @DatasetId
        ORDER BY CreatedAt DESC";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DatasetId", datasetId);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            versions.Add(new DatasetVersionDetailDto
                            {
                                DatasetVersionId = reader.GetGuid(reader.GetOrdinal("DatasetVersionId")),
                                VersionLabel = reader.GetString(reader.GetOrdinal("VersionLabel")),
                                FileFormat = reader.GetString(reader.GetOrdinal("FileFormat")),
                                PricePerDownload = reader.IsDBNull(reader.GetOrdinal("PricePerDownload")) ? null : reader.GetDecimal(reader.GetOrdinal("PricePerDownload")),

                                // ÁNH XẠ CÁC TRƯỜNG ĐÃ SỬA VÀ BỔ SUNG
                                HasSubscriptionOption = reader.GetBoolean(reader.GetOrdinal("HasSubscriptionOption")), // Dùng tên Alias
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                FilesizeBytes = reader.IsDBNull(reader.GetOrdinal("FilesizeBytes")) ? null : (long?)reader.GetInt64(reader.GetOrdinal("FilesizeBytes")),
                                StorageUri = reader.IsDBNull(reader.GetOrdinal("StorageUri")) ? null : reader.GetString(reader.GetOrdinal("StorageUri")),
                                IsAnalyzed = reader.GetBoolean(reader.GetOrdinal("IsAnalyzed")),
                                AnalysisReportUri = reader.IsDBNull(reader.GetOrdinal("AnalysisReportUri")) ? null : reader.GetString(reader.GetOrdinal("AnalysisReportUri")),
                                SampleUri = reader.IsDBNull(reader.GetOrdinal("SampleUri")) ? null : reader.GetString(reader.GetOrdinal("SampleUri")),
                                PricePerGB = reader.IsDBNull(reader.GetOrdinal("PricePerGB")) ? null : reader.GetDecimal(reader.GetOrdinal("PricePerGB")),
                                AccessPolicyId = reader.IsDBNull(reader.GetOrdinal("AccessPolicyId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("AccessPolicyId")),
                                LicenseText = reader.IsDBNull(reader.GetOrdinal("LicenseText")) ? null : reader.GetString(reader.GetOrdinal("LicenseText"))
                            });
                        }
                    }
                }
            }
            return versions;
        }
        // --- Phương thức 3: Lấy các DataFiles theo Version ---
        public async Task<IEnumerable<DataFileDto>> GetDataFilesByVersionIdAsync(Guid datasetVersionId)
        {
            var files = new List<DataFileDto>();
            var connectionString = _context.Database.GetConnectionString();
                
            string sql = @"
        SELECT 
            FileId AS DataFileId, DatasetVersionId, FileName, FileUri, FileSizeBytes, Checksum
        FROM DatasetFiles
        WHERE DatasetVersionId = @DatasetVersionId";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DatasetVersionId", datasetVersionId);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            files.Add(new DataFileDto
                            {
                                DataFileId = reader.GetGuid(reader.GetOrdinal("DataFileId")),
                                DatasetVersionId = reader.GetGuid(reader.GetOrdinal("DatasetVersionId")),
                                FileName = reader.GetString(reader.GetOrdinal("FileName")),
                                FileUri = reader.GetString(reader.GetOrdinal("FileUri")),
                                FileSizeBytes = reader.IsDBNull(reader.GetOrdinal("FileSizeBytes")) ? null : (long?)reader.GetInt64(reader.GetOrdinal("FileSizeBytes")),
                                Checksum = reader.IsDBNull(reader.GetOrdinal("Checksum")) ? null : reader.GetString(reader.GetOrdinal("Checksum"))
                            });
                        }
                    }
                }
            }
            return files;
        }
    }
    }
