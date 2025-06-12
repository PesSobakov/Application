using System.Text.Json.Serialization;

namespace Application.Server.Models.DTOs
{
    public class BookingQuestionResponseDto
    {
        [JsonPropertyName("type")] 
        public string Type { get; set; } = null!;
        [JsonPropertyName("count")]
        public int? Count { get; set; }
        [JsonPropertyName("data")]
        public List<int>? Data { get; set; }
    }
}
