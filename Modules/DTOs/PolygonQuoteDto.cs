using System.Text.Json.Serialization;

namespace npascu_api_v1.Modules.DTOs;

public class PolygonQuoteResponse
{
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("last")] public PolygonQuoteDto? Last { get; set; }
}

public class PolygonQuoteDto
{
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("size")] public long? Size { get; set; }
    [JsonPropertyName("exchange")] public int? Exchange { get; set; }
    [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
}
