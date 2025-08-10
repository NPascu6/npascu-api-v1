namespace Domain.DTOs;

/// <summary>
/// Standardized order book representation for downstream clients.
/// </summary>
public class OrderBookDto
{
    /// <summary>Normalized symbol (e.g. NASDAQ:AAPL).</summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>Server timestamp in unix milliseconds.</summary>
    public long Ts { get; set; }

    /// <summary>Bids sorted descending by price.</summary>
    public List<Level> Bids { get; set; } = new();

    /// <summary>Asks sorted ascending by price.</summary>
    public List<Level> Asks { get; set; } = new();

    /// <summary>Depth actually returned.</summary>
    public int Depth { get; set; }
}

/// <summary>
/// Price level information.
/// </summary>
public class Level
{
    public decimal Price { get; set; }
    public decimal Size { get; set; }
}
