using CsvHelper;
using EV.AdminService.API.AI.Data;
using EV.AdminService.API.AI.DataMap;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using System.Globalization;

namespace EV.AdminService.API.AI.Services.BackgroundServices
{
    public class DataQualityProcessor : BackgroundService
    {
        private readonly ILogger<DataQualityProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DataQualityProcessor(ILogger<DataQualityProcessor> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                    if (pendingVersion != null)
                    {
                        _logger.LogInformation("Processing VersionId: {VersionId}", pendingVersion.DatasetVersionId);
                        List<EVDataPoint> dataPoints = await LoadDataFromFileAsync(pendingVersion.StorageUri).ConfigureAwait(false);

                        var piiFlags = await servicesProvider.DataAnalysisService.DetectPiiAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var spikeFlags = await servicesProvider.DataAnalysisService.DetectSpikesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var changePointFlags = await servicesProvider.DataAnalysisService.DetectChangePointsAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var cheatingFlags = await servicesProvider.DataAnalysisService.DetectCheatingRulesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);

                        var allFlags = piiFlags.Concat(spikeFlags).Concat(changePointFlags).Concat(cheatingFlags).ToList();
                        await unitOfWork.DataQualityFlagRepository.AddRangeAsync(allFlags, stoppingToken);

                        if (!allFlags.Any())
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

                        await unitOfWork.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        _logger.LogInformation("Processed VersionId Successfully: {VersionId}", pendingVersion.DatasetVersionId);
                    }
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

        private async Task<List<EVDataPoint>> LoadDataFromFileAsync(string fileUri)
        {
            if (string.IsNullOrWhiteSpace(fileUri))
            {
                _logger.LogWarning("Empty StorageUri for data version.");
                return new List<EVDataPoint>();
            }

            try
            {
                _logger.LogInformation("Loading data from {Uri}", fileUri);
                using var http = new System.Net.Http.HttpClient();
                var csvContent = await http.GetStringAsync(fileUri).ConfigureAwait(false);

                using var reader = new StringReader(csvContent);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<EVDataPointMap>();
                var records = csv.GetRecords<EVDataPoint>().ToList();
                _logger.LogInformation("Loaded {Count} records from {Uri}", records.Count, fileUri);
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load data from {Uri}", fileUri);
                return new List<EVDataPoint>();
            }
        }
    }
}