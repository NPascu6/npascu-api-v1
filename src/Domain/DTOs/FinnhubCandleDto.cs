namespace Domain.DTOs;

/// <summary>Represents OHLC candle data from Finnhub.</summary>
public class FinnhubCandleDto
{
    public IEnumerable<decimal> c { get; set; } = Enumerable.Empty<decimal>();
    public IEnumerable<decimal> h { get; set; } = Enumerable.Empty<decimal>();
    public IEnumerable<decimal> l { get; set; } = Enumerable.Empty<decimal>();
    public IEnumerable<decimal> o { get; set; } = Enumerable.Empty<decimal>();
    public IEnumerable<long> t { get; set; } = Enumerable.Empty<long>();
    public string s { get; set; } = string.Empty;
}
