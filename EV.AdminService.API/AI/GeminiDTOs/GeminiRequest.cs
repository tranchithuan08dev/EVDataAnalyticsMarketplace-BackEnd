using System.Text.Json.Serialization;

namespace EV.AdminService.API.AI.GeminiDTOs
{
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = new List<GeminiContent>();
    }
}
