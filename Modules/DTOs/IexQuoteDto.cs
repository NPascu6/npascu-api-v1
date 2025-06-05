using System.Text.Json.Serialization;

namespace npascu_api_v1.Modules.DTOs;

public class IexQuoteDto
{
    [JsonPropertyName("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonPropertyName("latestPrice")] public decimal LatestPrice { get; set; }
    [JsonPropertyName("open")] public decimal? Open { get; set; }
    [JsonPropertyName("high")] public decimal? High { get; set; }
    [JsonPropertyName("low")] public decimal? Low { get; set; }
    [JsonPropertyName("previousClose")] public decimal? PreviousClose { get; set; }
    [JsonPropertyName("volume")] public long? Volume { get; set; }
}
