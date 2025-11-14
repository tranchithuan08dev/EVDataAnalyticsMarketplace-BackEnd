using System.Text.Json.Serialization;

namespace EV.AdminService.API.AI.GeminiDTOs
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate> Candidates { get; set; } = new List<GeminiCandidate>();
    }
}
