using System.Text.Json.Serialization;

namespace npascu_api_v1.Modules.DTOs;

public class YahooQuoteResponse
{
    [JsonPropertyName("quoteResponse")] public YahooQuoteWrapper? QuoteResponse { get; set; }
}

public class YahooQuoteWrapper
{
    [JsonPropertyName("result")] public List<YahooQuoteDto>? Result { get; set; }
}

public class YahooQuoteDto
{
    [JsonPropertyName("symbol")] public string Symbol { get; set; } = string.Empty;
    [JsonPropertyName("regularMarketPrice")] public decimal RegularMarketPrice { get; set; }
    [JsonPropertyName("regularMarketOpen")] public decimal? RegularMarketOpen { get; set; }
    [JsonPropertyName("regularMarketDayHigh")] public decimal? RegularMarketDayHigh { get; set; }
    [JsonPropertyName("regularMarketDayLow")] public decimal? RegularMarketDayLow { get; set; }
    [JsonPropertyName("regularMarketPreviousClose")] public decimal? RegularMarketPreviousClose { get; set; }
}
