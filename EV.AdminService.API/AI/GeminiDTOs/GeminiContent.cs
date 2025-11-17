using System.Text.Json.Serialization;

namespace EV.AdminService.API.AI.GeminiDTOs
{
    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = new List<GeminiPart>();
    }
}
