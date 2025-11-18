using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using EV.AdminService.API.AI.Data;
using EV.AdminService.API.AI.DataMap;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using OfficeOpenXml;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace EV.AdminService.API.AI.Services.BackgroundServices
{
    public class DataQualityProcessor : BackgroundService
    {
        private readonly ILogger<DataQualityProcessor> _logger;
        private readonly IAmazonS3 _s3Client;
        private readonly IServiceProvider _serviceProvider;

        public DataQualityProcessor(ILogger<DataQualityProcessor> logger, IServiceProvider serviceProvider, IAmazonS3 s3Client)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _s3Client = s3Client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("DataQualityProcessor is running at: {time}", DateTimeOffset.Now);
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var servicesProvider = scope.ServiceProvider.GetRequiredService<IServicesProvider>();

                    var pendingVersion = await unitOfWork.DatasetVersionRepository.GetPendingVersion(stoppingToken).ConfigureAwait(false);
                    if (pendingVersion == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
                        continue;
                    }

                    _logger.LogInformation("Processing VersionId: {VersionId}", pendingVersion.DatasetVersionId);
                    List<EVDataPoint> dataPoints = await LoadDataFromFileAsync(pendingVersion.StorageUri, pendingVersion.FileFormat, stoppingToken).ConfigureAwait(false);
                    if (!dataPoints.Any())
                    {
                        continue;
                    }

                    var piiFlags = await servicesProvider.DataAnalysisService.DetectPiiAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                    var spikeFlags = await servicesProvider.DataAnalysisService.DetectSpikesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                    var changePointFlags = await servicesProvider.DataAnalysisService.DetectChangePointsAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                    var cheatingFlags = await servicesProvider.DataAnalysisService.DetectCheatingRulesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);

                    var allFlags = piiFlags.Concat(spikeFlags).Concat(changePointFlags).Concat(cheatingFlags).ToList();
                    if (allFlags.Any())
                    {
                        await unitOfWork.DataQualityFlagRepository.AddRangeAsync(allFlags, stoppingToken);
                    }
                    else
                    {
                        await unitOfWork.DataQualityFlagRepository.CreateAsync(new DataQualityFlag
                        {
                            DatasetVersionId = pendingVersion.DatasetVersionId,
                            FlagType = "green",
                            Message = "Quét tự động hoàn tất, không phát hiện bất thường.",
                            ProcessedBy = "ML-Scanner",
                            CreatedAt = DateTime.UtcNow
                        }).ConfigureAwait(false);
                    }

                    _logger.LogInformation("Task #1: Anonymizing data for {VersionId}", pendingVersion.DatasetVersionId);

                    var cleanData = await servicesProvider.DataAnalysisService.AnonymizeDataAsync(dataPoints, stoppingToken).ConfigureAwait(false);

                    await unitOfWork.AnonymizationLogRepository.CreateAsync(new AnonymizationLog
                    {
                        DatasetVersionId = pendingVersion.DatasetVersionId,
                        Method = "Redaction/Hashing",
                        PerformedAt = DateTime.UtcNow
                    }, stoppingToken).ConfigureAwait(false);

                    _logger.LogInformation("Task #2: Generating summary report for {VersionId}", pendingVersion.DatasetVersionId);

                    var summaryReport = await servicesProvider.DataAnalysisService.GenerateSummaryReportAsync(cleanData, stoppingToken).ConfigureAwait(false);

                    var reportJson = JsonSerializer.Serialize(summaryReport);

                    await unitOfWork.AnalysisRepository.CreateAsync(new Analysis
                    {
                        AnalysisId = Guid.NewGuid(),
                        DatasetVersionId = pendingVersion.DatasetVersionId,
                        Title = $"Báo cáo phân tích tóm tắt (Tự động)",
                        Description = reportJson,
                        CreatedAt = DateTime.UtcNow,
                        Visibility = "private"
                    }, stoppingToken).ConfigureAwait(false);

                    _logger.LogInformation("Task #3: Suggesting price for {VersionId}", pendingVersion.DatasetVersionId);
                    var suggestedPrice = await servicesProvider.DataAnalysisService.SuggestPricingAsync(summaryReport, stoppingToken).ConfigureAwait(false);
                    pendingVersion.PricePerDownload = suggestedPrice.BuyPrice;
                    pendingVersion.PricePerGb = suggestedPrice.RentPrice;
                    pendingVersion.SubscriptionRequired = true;
                    _logger.LogInformation("Suggested prices for {VersionId}: Buy (Snapshot) = {Buy}, Rent (Subscription) = {Rent}",
                            pendingVersion.DatasetVersionId, suggestedPrice.BuyPrice, suggestedPrice.RentPrice);

                    await unitOfWork.DatasetVersionRepository.UpdateAsync(pendingVersion);
                    await unitOfWork.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                    _logger.LogInformation("Processed VersionId Successfully: {VersionId}", pendingVersion.DatasetVersionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in DataQualityProcessor");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException) { /* graceful exit */ }
            }
        }

        private async Task<List<EVDataPoint>> LoadDataFromFileAsync(string fileUri, string fileFormat, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileUri))
            {
                _logger.LogWarning("Empty StorageUri for data version.");
                return new List<EVDataPoint>();
            }

            try
            {
                _logger.LogInformation("Loading data from {Uri} using IAmazonS3 client...", fileUri);

                var uri = new Uri(fileUri);
                var pathSegments = uri.AbsolutePath.Trim('/').Split('/');

                var bucketName = pathSegments[0];
                var fileKey = string.Join("/", pathSegments.Skip(1));

                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey
                };

                using var response = await _s3Client.GetObjectAsync(request, ct).ConfigureAwait(false);
                using var stream = response.ResponseStream;

                var records = new List<EVDataPoint>();

                if (fileFormat.Equals("csv", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Reading .csv file with CsvHelper...");
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    // ... (Code CsvHelper) ...
                    csv.Context.RegisterClassMap<EVDataPointMap>();
                    records = csv.GetRecords<EVDataPoint>().ToList();
                }
                else if (fileFormat.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Reading .xlsx file with EPPlus...");
                    using (var package = new ExcelPackage(stream))
                    {
                        var sheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (sheet == null)
                        {
                            _logger.LogWarning("No worksheet found in the Excel file.");
                            return records;
                        }

                        int startRow = 2;
                        for (int row = startRow; row <= sheet.Dimension.End.Row; row++)
                        {
                            try
                            {
                                var record = new EVDataPoint
                                {
                                    Timestamp = sheet.Cells[row, 1].GetValue<string>(),
                                    SoC = sheet.Cells[row, 2].GetValue<float>(),
                                    BatteryTemp = sheet.Cells[row, 3].GetValue<float>(),
                                    Odometer = sheet.Cells[row, 4].GetValue<float>(),
                                    DriverNotes = sheet.Cells[row, 5].GetValue<string>()
                                };
                                records.Add(record);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Lỗi đọc hàng {Row} từ file .xlsx.", row);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("Not supported file format: {Format}", fileFormat);
                    return new List<EVDataPoint>();
                }

                _logger.LogInformation("Loaded {Count} records from {Uri}", records.Count, fileUri);
                return records;
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "LỖI S3 KHI TẢI XUỐNG: File không thể truy cập (403 Forbidden). Rsp: {Msg}", s3Ex.Message);
                return new List<EVDataPoint>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load data from {Uri}", fileUri);
                return new List<EVDataPoint>();
            }
        }
    }
}