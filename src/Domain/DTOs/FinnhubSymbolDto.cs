namespace Domain.DTOs;

/// <summary>Represents a tradeable instrument returned from Finnhub.</summary>
public class FinnhubSymbolDto
{
    public string symbol { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string? exchange { get; set; }
}
