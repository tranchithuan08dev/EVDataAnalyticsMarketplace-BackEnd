using EV.AdminService.API.AI.Data;
using EV.AdminService.API.AI.Services.Interfaces;
using EV.AdminService.API.Models;
using Microsoft.ML;
using System.Text.RegularExpressions;

namespace EV.AdminService.API.AI.Services.Implements
{
    public class DataAnalysisService : IDataAnalysisService
    {
        private readonly MLContext _context;
        private static readonly Regex EmailRegex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]{2,4}", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"(\+\d{1,3})?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}", RegexOptions.Compiled);

        public DataAnalysisService(MLContext context)
        {
            _context = context;
        }

        public async Task<List<DataQualityFlag>> DetectChangePointsAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var flags = new List<DataQualityFlag>();

                int trainingWindowSize = dataPoints.Count / 2; // Ví dụ: lấy 1/2 dữ liệu để huấn luyện
                int historyWindowSize = 10;

                if (dataPoints.Count < 20)
                {
                    return flags; //Không đủ data, return empty
                }

                IDataView dataView = _context.Data.LoadFromEnumerable(dataPoints);
                var pipeline = _context.Transforms.DetectChangePointBySsa(
                    nameof(TimeSeriesPrediction.Prediction),
                    nameof(EVDataPoint.BatteryTemp),
                    confidence: 99.0,
                    changeHistoryLength: historyWindowSize,
                    trainingWindowSize: trainingWindowSize,
                    seasonalityWindowSize: 2);

                IDataView transformedData = pipeline.Fit(dataView).Transform(dataView);
                var predictions = _context.Data.CreateEnumerable<TimeSeriesPrediction>(transformedData, false).ToList();

                for (int i = 0; i < predictions.Count; i++)
                {
                    if (predictions[i].Prediction[0] == 1)
                    {
                        if (predictions[i].Prediction[0] == 1)

                            flags.Add(CreateFlag(versionId, 
                                                "red", 
                                                $"Phát hiện Change Point (BatteryTemp) tại dòng {i + 1}. Giá trị = {dataPoints[i].BatteryTemp}", 
                                                "ML-ChangePoint-Detector"));

                    }
                }
                return flags;
            }, ct).ConfigureAwait(false);
        }

        public async Task<List<DataQualityFlag>> DetectCheatingRulesAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var flags = new List<DataQualityFlag>();

                for (int i = 1; i < dataPoints.Count; i++)
                {
                    if (dataPoints[i].Odometer < dataPoints[i - 1].Odometer)
                        flags.Add(CreateFlag(versionId, 
                                            "red", 
                                            $"Phát hiện gian lận (Odometer) tại dòng {i + 1}. Giá trị giảm từ {dataPoints[i - 1].Odometer} xuống {dataPoints[i].Odometer}", 
                                            "Rule-Cheating-Detector"));
                }

                return flags;

            }, ct).ConfigureAwait(false);
        }

        public async Task<List<DataQualityFlag>> DetectPiiAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var flags = new List<DataQualityFlag>();

                for (int i = 0; i < dataPoints.Count; i++)
                {
                    var note = dataPoints[i].DriverNotes;
                    if (string.IsNullOrEmpty(note)) continue;

                    if (EmailRegex.IsMatch(note))
                    {
                        flags.Add(CreateFlag(versionId,
                                            "red",
                                            $"Phát hiện PII (Email) tại dòng {i + 1}",
                                            "PII-Email-Detector"));
                    }
                    if (PhoneRegex.IsMatch(note))
                    {
                        flags.Add(CreateFlag(versionId,
                                            "red",
                                            $"Phát hiện PII (SĐT) tại dòng {i + 1}",
                                            "PII-Phone-Detector"));
                    }
                }

                return flags;

            }, ct).ConfigureAwait(false);
        }

        public async Task<List<DataQualityFlag>> DetectSpikesAsync(Guid versionId, List<EVDataPoint> dataPoints, CancellationToken ct)
        {
            return await Task.Run(() =>
            {
                var flags = new List<DataQualityFlag>();

                // 1. Đảm bảo giá trị tối thiểu là 1 (để tránh lỗi chia cho 0)
                int calculatedLength = Math.Max(1, dataPoints.Count / 4);

                // 2. Áp dụng giới hạn tối đa là 30
                int pValueHistoryLength = Math.Min(30, calculatedLength);

                // Thuật toán yêu cầu số điểm dữ liệu phải lớn hơn cửa sổ lịch sử.
                if (dataPoints.Count <= pValueHistoryLength)
                {
                    return flags; //Không đủ data, return empty
                }

                IDataView dataView = _context.Data.LoadFromEnumerable(dataPoints);
                var pipeline = _context.Transforms.DetectIidSpike(
                    nameof(TimeSeriesPrediction.Prediction),
                    nameof(EVDataPoint.SoC),
                    confidence: 99.0,
                    pvalueHistoryLength: pValueHistoryLength,
                    side: Microsoft.ML.Transforms.TimeSeries.AnomalySide.TwoSided);

                IDataView transformedData = pipeline.Fit(dataView).Transform(dataView);
                var predictions = _context.Data.CreateEnumerable<TimeSeriesPrediction>(transformedData, false).ToList();

                for (int i = 0; i < predictions.Count; i++)
                {
                    if (predictions[i].Prediction[0] == 1)
                    {
                        flags.Add(CreateFlag(versionId,
                                            "yellow",
                                            $"Phát hiện Spike (SoC) tại dòng {i + 1}. Giá trị = {dataPoints[i].SoC}",
                                            "ML-Spike-Detector"));
                    }
                }

                return flags;
            }, ct).ConfigureAwait(false);
        }

        private DataQualityFlag CreateFlag(Guid versionId, string type, string message, string processor)
        {
            return new DataQualityFlag
            {
                DatasetVersionId = versionId,
                FlagType = type,
                Message = message,
                ProcessedBy = processor,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
