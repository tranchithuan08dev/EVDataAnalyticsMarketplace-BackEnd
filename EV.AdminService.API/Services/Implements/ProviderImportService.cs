using Amazon.S3;
using Amazon.S3.Transfer;
using EV.AdminService.API.DTOs.DataModels;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Interfaces;
using OfficeOpenXml;

namespace EV.AdminService.API.Services.Implements
{
    public class ProviderImportService : IProviderImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        public ProviderImportService(IUnitOfWork unitOfWork, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _s3Client = s3Client;
            _configuration = configuration;
        }
        public async Task<Guid> ImportNewDatasetAsync(IFormFile metadataFile, IFormFile dataFile, Guid providerId, CancellationToken ct)
        {
            string storageUri = string.Empty;
            var fileKey = $"evdata/provider_{providerId}/{Guid.NewGuid()}/{dataFile.FileName}";
            var bucketName = _configuration["CloudflareR2:BucketName"];

            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);
                await using var stream = dataFile.OpenReadStream();
                await fileTransferUtility.UploadAsync(stream, bucketName, fileKey, ct).ConfigureAwait(false);

                storageUri = $"{_configuration["CloudflareR2:EndpointUrl"]}/{bucketName}/{fileKey}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Upload file dữ liệu thất bại.", ex);
            }

            var datasetDto = new DatasetMetadataDTO();
            var versionDto = new VersionMetadataDTO();

            try
            {
                using (var package = new ExcelPackage(metadataFile.OpenReadStream()))
                {
                    var sheet1 = package.Workbook.Worksheets["Dataset"];
                    datasetDto.Title = sheet1.Cells["A2"].GetValue<string>();
                    datasetDto.ShortDescription = sheet1.Cells["B2"].GetValue<string>();
                    datasetDto.Category = sheet1.Cells["C2"].GetValue<string>();
                    datasetDto.Region = sheet1.Cells["D2"].GetValue<string>();
                    datasetDto.VehicleTypes = sheet1.Cells["E2"].GetValue<string>();

                    var sheet2 = package.Workbook.Worksheets["Version"];
                    versionDto.VersionLabel = sheet2.Cells["A2"].GetValue<string>();
                    versionDto.FileFormat = Path.GetExtension(dataFile.FileName).TrimStart('.');
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("File metadata (.xlsx) không đúng định dạng hoặc thiếu sheet.", ex);
            }

            var newDataset = new Dataset
            {
                ProviderId = providerId,
                Title = datasetDto.Title,
                ShortDescription = datasetDto.ShortDescription,
                Category = datasetDto.Category,
                Region = datasetDto.Region,
                VehicleTypes = datasetDto.VehicleTypes,
                CreatedAt = DateTime.UtcNow,
                Status = "pending",
                Visibility = "private",
            };

            await _unitOfWork.DatasetRepository.CreateAsync(newDataset, ct);

            var newVersion = new DatasetVersion
            {
                DatasetId = newDataset.DatasetId,
                VersionLabel = versionDto.VersionLabel,
                FileFormat = versionDto.FileFormat,
                StorageUri = storageUri,
                CreatedAt = DateTime.UtcNow,
                IsAnalyzed = false,
                SubscriptionRequired = false,
            };

            await _unitOfWork.DatasetVersionRepository.CreateAsync(newVersion, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
            return newDataset.DatasetId;
        }
    }
}
