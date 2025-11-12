using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EV.DataConsumerService.API.Data.Repositories
{
    public class DatasetRepository : IDatasetRepository
    {

        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public DatasetRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        public IQueryable<Dataset> FindAllPublicDatasets()
        {
            return _context.Datasets
            .Where(d => d.Status == "approved" && d.Visibility == "public")
            .Include(d => d.DatasetVersions)
            .AsNoTracking(); 
        }

        

        public IQueryable<Dataset> GetFullPublicDatasetsQuery()
        {
            return _context.Datasets
             .Include(d => d.DatasetVersions)
             .Where(d => d.Visibility == "public" && d.Status == "approved");
        }


        public async Task<IEnumerable<DatasetSearchDetailDto>> SearchDatasetsAsync(DatasetSearchRequestDto searchRequest)
        {
            var results = new List<DatasetSearchDetailDto>();

            // Lấy chuỗi kết nối
            var connectionString = _context.Database.GetConnectionString();

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand("usp_SearchDatasets", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số (Parameter)
                    command.Parameters.AddWithValue("@q", (object)searchRequest.Q ?? DBNull.Value);
                    command.Parameters.AddWithValue("@category", (object)searchRequest.Category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@region", (object)searchRequest.Region ?? DBNull.Value);
                    command.Parameters.AddWithValue("@vehicleType", (object)searchRequest.VehicleType ?? DBNull.Value);

                    // Xử lý giá trị Nullable
                    command.Parameters.AddWithValue("@minPrice", searchRequest.MinPrice.HasValue ? (object)searchRequest.MinPrice.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@maxPrice", searchRequest.MaxPrice.HasValue ? (object)searchRequest.MaxPrice.Value : DBNull.Value);

                    // Tham số phân trang
                    command.Parameters.AddWithValue("@page", searchRequest.Page);
                    command.Parameters.AddWithValue("@pageSize", searchRequest.PageSize);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new DatasetSearchDetailDto
                            {
                                DatasetId = reader.GetGuid(reader.GetOrdinal("DatasetId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                // Sử dụng GetOrdinal và kiểm tra DBNull an toàn hơn
                                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader.GetString(reader.GetOrdinal("Region")),
                                VehicleTypes = reader.IsDBNull(reader.GetOrdinal("VehicleTypes")) ? null : reader.GetString(reader.GetOrdinal("VehicleTypes")),
                                DatasetVersionId = reader.GetGuid(reader.GetOrdinal("DatasetVersionId")),
                                VersionLabel = reader.GetString(reader.GetOrdinal("VersionLabel")),
                                FileFormat = reader.GetString(reader.GetOrdinal("FileFormat")),
                                PricePerDownload = reader.IsDBNull(reader.GetOrdinal("PricePerDownload")) ? null : reader.GetDecimal(reader.GetOrdinal("PricePerDownload"))
                            });
                        }
                    }
                }
            }
            return results;
        }

        public async Task ExecutePurchaseAsync(PurchaseRequestDto purchaseRequest)
        {
            var connectionString = _context.Database.GetConnectionString();

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand("usp_PurchaseDataset", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số cho Stored Procedure:
                    command.Parameters.AddWithValue("@BuyerUserId", purchaseRequest.BuyerUserId);
                    command.Parameters.AddWithValue("@BuyerOrgId", purchaseRequest.BuyerOrgId);
                    command.Parameters.AddWithValue("@DatasetVersionId", purchaseRequest.DatasetVersionId);
                    command.Parameters.AddWithValue("@Price", purchaseRequest.Price);
                    // Tham số tùy chọn:
                    command.Parameters.AddWithValue("@Currency", (object)purchaseRequest.Currency ?? "USD");
                    command.Parameters.AddWithValue("@AccessDays", purchaseRequest.AccessDays.HasValue ? (object)purchaseRequest.AccessDays.Value : DBNull.Value);

                    // Thực thi Stored Procedure (không cần đọc kết quả, chỉ cần thực thi)
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<SubscriptionResponseDto> ExecuteSubscriptionAndApiKeyCreationAsync(
                SubscriptionRequestDto request,
                byte[] apiKeyHash,
                DateTime expiresAt,
                string plainApiKey)
        {
            var connectionString = _context.Database.GetConnectionString();
            var response = new SubscriptionResponseDto();

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                await using (SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. CHÈN VÀO BẢNG SUBSCRIPTIONS
                        Guid subscriptionId = Guid.NewGuid();
                        string subSql = @"
                            INSERT INTO Subscriptions (SubscriptionId, ConsumerOrgId, DatasetId, ExpiresAt, RecurringPrice)
                            VALUES (@SubscriptionId, @ConsumerOrgId, @DatasetId, @ExpiresAt, @RecurringPrice)";

                        // Không cần ép kiểu transaction trong constructor nữa
                        await using (var subCommand = new SqlCommand(subSql, connection, transaction))
                        {
                            subCommand.Parameters.AddWithValue("@SubscriptionId", subscriptionId);
                            subCommand.Parameters.AddWithValue("@ConsumerOrgId", request.ConsumerOrgId);
                            subCommand.Parameters.AddWithValue("@DatasetId", request.DatasetId);
                            subCommand.Parameters.AddWithValue("@ExpiresAt", expiresAt);
                            subCommand.Parameters.AddWithValue("@RecurringPrice", request.RecurringPrice);
                            await subCommand.ExecuteNonQueryAsync();

                            response.SubscriptionId = subscriptionId;
                        }

                        // 2. CHÈN VÀO BẢNG APIKEYS
                        Guid apiKeyId = Guid.NewGuid();
                        string keySql = @"
                            INSERT INTO ApiKeys (ApiKeyId, OrganizationId, KeyHash, Description, ExpiresAt)
                            VALUES (@ApiKeyId, @OrganizationId, @KeyHash, @Description, @ExpiresAt)";

                        // Không cần ép kiểu transaction trong constructor nữa
                        await using (var keyCommand = new SqlCommand(keySql, connection, transaction))
                        {
                            keyCommand.Parameters.AddWithValue("@ApiKeyId", apiKeyId);
                            keyCommand.Parameters.AddWithValue("@OrganizationId", request.ConsumerOrgId);
                            keyCommand.Parameters.AddWithValue("@KeyHash", apiKeyHash);
                            keyCommand.Parameters.AddWithValue("@Description", $"API Key cho thuê bao Dataset {request.DatasetId}");
                            keyCommand.Parameters.AddWithValue("@ExpiresAt", expiresAt);
                            await keyCommand.ExecuteNonQueryAsync();

                            response.ApiKeyId = apiKeyId;
                            response.NewApiKey = plainApiKey;
                            response.ExpiresAt = expiresAt;
                            response.Message = "Thuê bao và API Key đã được tạo thành công.";
                        }

                        await transaction.CommitAsync();
                        return response;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Lỗi khi tạo thuê bao và API Key trong giao dịch.", ex);
                    }
                }
            }
        
    }

    }
}
