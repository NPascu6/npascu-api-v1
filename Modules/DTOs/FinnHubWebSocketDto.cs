using System.Text.Json.Serialization;

namespace npascu_api_v1.Modules.DTOs
{
    public class FinnhubWebSocketResponse
    {
        [JsonPropertyName("data")] public List<FinnhubTradeDto>? Data { get; set; }

        [JsonPropertyName("type")] public string? Type { get; set; }
    }

    public class FinnhubTradeDto
    {
        // Price (p)
        [JsonPropertyName("p")] public decimal Price { get; set; }

        // Symbol (s)
        [JsonPropertyName("s")] public string Symbol { get; set; } = string.Empty;

        // Timestamp (t)
        [JsonPropertyName("t")] public long Timestamp { get; set; }

        // Volume (v) - changed from int to decimal
        [JsonPropertyName("v")] public decimal Volume { get; set; }

        // (Optional) If you need to capture the 'c' field, you can add:
        [JsonPropertyName("c")] public decimal? Condition { get; set; }
    }
}