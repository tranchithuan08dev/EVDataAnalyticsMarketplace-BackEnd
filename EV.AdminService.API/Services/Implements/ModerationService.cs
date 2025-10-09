using EV.AdminService.API.DTOs.Result;
using EV.AdminService.API.Services.Interfaces;

namespace EV.AdminService.API.Services.Implements
{
    public class ModerationService : IModerationService
    {
        private readonly HttpClient _httpClient;
        public ModerationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ModerationResult> AnalyzeDatasetAsync(Guid datasetId, string title, string shortDescription, CancellationToken ct = default)
        {
            try
            {
                var payload = new {
                    DatasetId = datasetId,
                    Title = title,
                    ShortDescription = shortDescription
                };

                var response = await _httpClient.PostAsJsonAsync("/api/moderate", payload, ct);

                if (response.IsSuccessStatusCode)
                {
                    var r = await response.Content.ReadFromJsonAsync<ModerationResult>(cancellationToken: ct);
                    if (r != null)
                    {
                        return r;
                    }
                }
            }
            catch
            {
            }

            return new ModerationResult
            {
                DatasetId = datasetId,
                Score = 0.0,
                Labels = new List<string> {},
                Explanation = "Fallback",
            };
        }
    }
}
