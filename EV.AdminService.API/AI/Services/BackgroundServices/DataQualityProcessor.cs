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
        private readonly IServicesProvider _servicesProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DataQualityProcessor(ILogger<DataQualityProcessor> logger, IServicesProvider servicesProvider, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _servicesProvider = servicesProvider;
            _unitOfWork = unitOfWork;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("DataQualityProcessor is running at: {time}", DateTimeOffset.Now);
                try
                {
                    var pendingVersion = await _unitOfWork.DatasetVersionRepository.GetPendingVersion(stoppingToken);
                    if (pendingVersion != null)
                    {
                        _logger.LogInformation($"Processing VersionId: {pendingVersion.DatasetVersionId}");
                        List<EVDataPoint> dataPoints = await LoadDataFromFileAsync(pendingVersion.StorageUri).ConfigureAwait(false);

                        var piiFlags = await _servicesProvider.DataAnalysisService.DetectPiiAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var spikeFlags = await _servicesProvider.DataAnalysisService.DetectSpikesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var changePointFlags = await _servicesProvider.DataAnalysisService.DetectChangePointsAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);
                        var cheatingFlags = await _servicesProvider.DataAnalysisService.DetectCheatingRulesAsync(pendingVersion.DatasetVersionId, dataPoints, stoppingToken).ConfigureAwait(false);

                        var allFlags = piiFlags.Concat(spikeFlags).Concat(changePointFlags).Concat(cheatingFlags).ToList();
                        await _unitOfWork.DataQualityFlagRepository.AddRangeAsync(allFlags, stoppingToken);

                        if (!allFlags.Any())
                        {
                            await _unitOfWork.DataQualityFlagRepository.CreateAsync(new DataQualityFlag
                            {
                                DatasetVersionId = pendingVersion.DatasetVersionId,
                                FlagType = "green",
                                Message = "Quét tự động hoàn tất, không phát hiện bất thường.",
                                ProcessedBy = "ML-Scanner",
                                CreatedAt = DateTime.UtcNow
                            }).ConfigureAwait(false);
                        }

                        await _unitOfWork.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        _logger.LogInformation($"Processed VersionId Successfully: {pendingVersion.DatasetVersionId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in DataQualityProcessor");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken).ConfigureAwait(false);
            }
        }

        private async Task<List<EVDataPoint>> LoadDataFromFileAsync(string fileUri)
        {
            return await Task.Run(() =>
            {
                var csvContent = @"2025-10-25T03:30:00Z,80.5,42.1,1024.5,Note 1
                               2025-10-25T03:30:05Z,95.0,42.4,1027.5,Note 2
                               2025-10-25T03:30:32Z,74.8,44.6,1030.0,Note 3
                               2025-10-25T03:31:01Z,69.9,0.0,1050.6,Email: test@example.com
                               2025-10-25T03:31:02Z,69.8,0.0,1051.2,SDT: (123) 456-7890";

                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<EVDataPointMap>();
                    return csv.GetRecords<EVDataPoint>().ToList();
                }
            }).ConfigureAwait(false);
        }
    }
}
