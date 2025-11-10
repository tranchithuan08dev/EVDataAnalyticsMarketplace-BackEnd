using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Models.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace EV.DataConsumerService.API.Service
{
    public class DatasetService : IDatasetService
    {
        private readonly IDatasetRepository _repository;

        // 1. Khai báo Metric Counter: Đếm số lần tìm kiếm được thực hiện
        private static readonly Counter DatasetSearchCount = Metrics
            .CreateCounter("marketplace_dataset_searches_total",
                           "Total number of times the public dataset search service was called.");

        // 2. Khai báo Metric Summary: Đo lường thời gian thực thi của Service
        private static readonly Summary DatasetSearchDuration = Metrics
            .CreateSummary("marketplace_dataset_search_duration_seconds",
                           "Duration of the GetPublicDatasetsForSearch service method.",
                           new SummaryConfiguration { MaxAge = TimeSpan.FromMinutes(5), Objectives = new[] { new QuantileEpsilonPair(0.5, 0.05), new QuantileEpsilonPair(0.9, 0.01), new QuantileEpsilonPair(0.99, 0.001) } });


        public DatasetService(IDatasetRepository repository)
        {
            _repository = repository;
        
        }
        [EnableQuery]
        public IQueryable<DatasetSearchResultDto> GetPublicDatasetsForSearch()
        {
            // Bắt đầu đo thời gian khi phương thức được gọi
            using (DatasetSearchDuration.NewTimer())
            {
                // Tăng bộ đếm mỗi khi hàm được gọi
                DatasetSearchCount.Inc();

                // ----------------------------------------------------
                // Lấy dữ liệu từ Repository (IQueryable<T> chưa thực thi)
                var datasets = _repository.FindAllPublicDatasets();

                // Ánh xạ IQueryable<T> sang DTO IQueryable<T>
                var results = datasets.Select(d => new DatasetSearchResultDto
                    {
                        DatasetId = d.DatasetId,
                        Title = d.Title,
                        Category = d.Category,
                        Region = d.Region,

                    // Logic làm phẳng (Flattening Logic)
                    // Lưu ý: Các hàm như Select(), Min(), Any() đều được dịch sang SQL khi truy vấn
                    FileFormat = d.DatasetVersions.Select(v => v.FileFormat).FirstOrDefault(),
                    // Sử dụng toán tử ?? 0 để đảm bảo giá trị không null cho metric (nếu PricePerDownload là null)
                    MinPricePerDownload = d.DatasetVersions.Min(v => v.PricePerDownload) ?? 0,
                    HasSubscriptionOption = d.DatasetVersions.Any(v => v.SubscriptionRequired)
                });
                // ----------------------------------------------------

                // Kết quả IQueryable<T> này được trả về Controller.
                // OData Middleware sẽ thực thi truy vấn tại đây và kết thúc đo thời gian (vì timer kết thúc).
                return results;
            }
        }

        public List<DatasetFullDetailDto> GetFullDatasetDetails()
        {
            // 1. Định nghĩa logic Retry (Thử lại 3 lần, chờ 5 giây giữa các lần)
            int maxRetries = 3;
            TimeSpan delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // 2. Lấy IQueryable<T> từ Repository
                    var query = _repository.GetFullPublicDatasetsQuery();

                    // 3. THỰC THI TRUY VẤN TẠI ĐÂY (Buộc truy vấn chạy)
                    var datasets = query.ToList();

                    // 4. Mapping (Nếu truy vấn thành công)
                    var result = datasets.Select(d => new DatasetFullDetailDto
                    {
                        // ... (Logic Mapping của bạn, giữ nguyên)
                        DatasetId = d.DatasetId,
                        Title = d.Title,
                        ShortDescription = d.ShortDescription,
                        Category = d.Category,
                        Region = d.Region,
                        VehicleTypes = d.VehicleTypes,
                        Status = d.Status,
                        Versions = d.DatasetVersions.Select(v => new DatasetVersionDto
                        {
                            DatasetVersionId = v.DatasetVersionId,
                            VersionLabel = v.VersionLabel,
                            FileFormat = v.FileFormat,
                            PricePerDownload = v.PricePerDownload,
                            SubscriptionRequired = v.SubscriptionRequired,
                            StorageUri = v.StorageUri
                        }).ToList()
                    }).ToList();

                    // Trả về nếu thành công
                    return result;
                }
                catch (Exception ex)
                {
                    // Chỉ retry nếu đó là lỗi liên quan đến kết nối SQL Server (lỗi mạng)
                    if (ex.InnerException?.Message.Contains("network-related") == true && i < maxRetries - 1)
                    {
                        // Log: "Kết nối DB thất bại. Thử lại sau 5 giây..."
                        Thread.Sleep(delay); // Dừng luồng hiện tại
                        continue; // Bắt đầu vòng lặp retry tiếp theo
                    }

                    // Nếu là lần thử cuối cùng hoặc lỗi không phải lỗi mạng, ném lỗi ra ngoài
                    throw;
                }
            }

            // Code không bao giờ tới đây, nhưng cần return để tránh lỗi cú pháp
            return new List<DatasetFullDetailDto>();
        }

        public async Task<IEnumerable<DatasetSearchDetailDto>> SearchDatasetsAsync(DatasetSearchRequestDto searchRequest)
        {
            return await _repository.SearchDatasetsAsync(searchRequest);
        }

    }
}

