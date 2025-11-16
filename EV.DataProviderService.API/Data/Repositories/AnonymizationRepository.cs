using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Models.Entites;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EV.DataProviderService.API.Repositories
{
    public class AnonymizationRepository : IAnonymizationRepository
    {
        private readonly EvdataAnalyticsMarketplaceDbContext _context;

        public AnonymizationRepository(EvdataAnalyticsMarketplaceDbContext context)
        {
            _context = context;
        }

        public async Task<AnonymizationLog> LogAnonymizationAsync(AnonymizationLog log)
        {
            _context.AnonymizationLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<bool> AnonymizeDatasetFilesAsync(Guid datasetVersionId, List<string> fields, string method)
        {
            var files = await _context.DatasetFiles
                .Where(f => f.DatasetVersionId == datasetVersionId)
                .ToListAsync();

            if (!files.Any()) return false;

            foreach (var file in files)
            {
                // Giả lập đọc file từ Storage (thực tế: Azure Blob, S3, MinIO)
                var content = await FakeReadFile(file.FileUri);
                var anonymized = AnonymizeContent(content, fields, method);
                await FakeWriteFile(file.FileUri, anonymized);
            }

            return true;
        }

        // === GIẢ LẬP I/O (thay bằng thực tế sau) ===
        private Task<string> FakeReadFile(string uri) => Task.FromResult(
            @"{""email"": ""user@example.com"", ""ip"": ""192.168.1.1"", ""name"": ""Nguyen Van A""}"
        );

        private Task FakeWriteFile(string uri, string content) => Task.CompletedTask;

        // === CÁC PHƯƠNG PHÁP ẨN DANH ===
        private string AnonymizeContent(string content, List<string> fields, string method)
        {
            return method switch
            {
                "Hash" => HashFields(content, fields),
                "Mask" => MaskFields(content, fields),
                "Delete" => DeleteFields(content, fields),
                "Replace" => ReplaceFields(content, fields),
                _ => content
            };
        }

        private string HashFields(string content, List<string> fields)
        {
            using var sha256 = SHA256.Create();
            foreach (var field in fields)
            {
                var regex = new System.Text.RegularExpressions.Regex($"(\"{field}\":\\s*\").*?(\")");
                content = regex.Replace(content, m =>
                {
                    var value = m.Groups[1].Value;
                    var hash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "");
                    return $"{m.Groups[1].Value}{hash}{m.Groups[2].Value}";
                });
            }
            return content;
        }

        private string MaskFields(string content, List<string> fields)
        {
            foreach (var field in fields)
            {
                var regex = new System.Text.RegularExpressions.Regex($"(\"{field}\":\\s*\").*?(\")");
                content = regex.Replace(content, "$1***$2");
            }
            return content;
        }

        private string DeleteFields(string content, List<string> fields)
        {
            foreach (var field in fields)
            {
                content = System.Text.RegularExpressions.Regex.Replace(content, $"\"{field}\":\\s*\".*?\"", "\"{field}\": null");
            }
            return content;
        }

        private string ReplaceFields(string content, List<string> fields)
        {
            foreach (var field in fields)
            {
                content = content.Replace(field switch
                {
                    "email" => "user@example.com",
                    "ip" => "192.168.1.1",
                    "name" => "Nguyen Van A",
                    _ => ""
                }, "anonymous");
            }
            return content;
        }
    }
}
