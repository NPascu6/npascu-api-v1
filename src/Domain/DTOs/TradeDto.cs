namespace Domain.DTOs;

/// <summary>
/// Represents a normalized trade tick.
/// </summary>
public class TradeDto
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Size { get; set; }
    public string? Side { get; set; }
    /// <summary>Unix timestamp in milliseconds.</summary>
    public long Ts { get; set; }
}
