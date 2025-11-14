using System.Text.Json.Serialization;

namespace EV.AdminService.API.AI.GeminiDTOs
{
    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }
}
