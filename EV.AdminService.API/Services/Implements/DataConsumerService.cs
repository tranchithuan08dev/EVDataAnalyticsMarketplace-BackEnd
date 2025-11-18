using Amazon.S3;
using Amazon.S3.Model;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV.AdminService.API.Services.Implements
{
    public class DataConsumerService : IDataConsumerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataConsumerService> _logger;
        public DataConsumerService(IUnitOfWork unitOfWork, IAmazonS3 s3Client, IConfiguration configuration, ILogger<DataConsumerService> logger)
        {
            _unitOfWork = unitOfWork;
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<DatasetVersionMetadataDTO>> GetDatasetVersionsAsync(Guid organizationId, Guid datasetId, CancellationToken ct)
        {
            bool hasAccess = await _unitOfWork.SubscriptionRepository.HasActiveSubscriptionAsync(organizationId, datasetId, ct);
            if (!hasAccess)
            {
                _logger.LogWarning("Từ chối danh sách: Org {OrgId} chưa thuê Dataset {DataId}", organizationId, datasetId);
                throw new UnauthorizedAccessException("Bạn chưa đăng ký (subscribe) gói dữ liệu này.");
            }

            var versions = await _unitOfWork.DatasetVersionRepository.GetApprovedVersionsAsync(datasetId, ct);

            return versions;
        }

        public async Task<string> GetSubscribedVersionLinkAsync(Guid organizationId, Guid datasetVersionId, CancellationToken ct)
        {
            var version = await _unitOfWork.DatasetVersionRepository.GetByIdAsync(ct, datasetVersionId);
            if (version == null) throw new KeyNotFoundException("Phiên bản không tồn tại.");

            bool hasAccess = await _unitOfWork.SubscriptionRepository
                .HasActiveSubscriptionAsync(organizationId, version.DatasetId, ct);

            if (!hasAccess)
            {
                _logger.LogWarning("Từ chối download (Thuê): Org {OrgId}, Version {VerId}", organizationId, datasetVersionId);
                throw new UnauthorizedAccessException("Bạn chưa đăng ký (subscribe) gói dữ liệu này.");
            }

            return GetSecureDownloadLink(version.StorageUri);
        }

        private string GetSecureDownloadLink(string storageUri)
        {
            if (string.IsNullOrEmpty(storageUri))
            {
                _logger.LogWarning("Không thể tạo link download: StorageUri bị rỗng.");
                throw new KeyNotFoundException("Không tìm thấy file dữ liệu (StorageUri rỗng).");
            }

            try
            {
                var bucketName = _configuration["CloudflareR2:BucketName"];
                var fileKey = new Uri(storageUri).AbsolutePath.TrimStart('/');
                if (fileKey.StartsWith(bucketName + "/"))
                {
                    fileKey = fileKey.Substring(bucketName!.Length + 1);
                }

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = fileKey,
                    Expires = DateTime.UtcNow.AddMinutes(10) // Link tồn tại 10 phút
                };

                return _s3Client.GetPreSignedURL(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo Pre-signed URL cho file: {FileUri}", storageUri);
                throw new InvalidOperationException("Không thể tạo link download.");
            }
        }
    }
}
