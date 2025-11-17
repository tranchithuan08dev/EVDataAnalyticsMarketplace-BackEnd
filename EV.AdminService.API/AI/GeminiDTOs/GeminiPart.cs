using System.Text.Json.Serialization;

namespace EV.AdminService.API.AI.GeminiDTOs
{
    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } =  string.Empty;
    }
}
