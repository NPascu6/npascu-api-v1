namespace Domain.DTOs;

/// <summary>
/// Represents trade ticks returned by Finnhub.
/// </summary>
public class FinnhubTradeDto
{
    /// <summary>
    /// Symbol for the trades.
    /// </summary>
    public string s { get; set; } = string.Empty;

    /// <summary>
    /// Collection of trade ticks.
    /// </summary>
    public IEnumerable<FinnhubTradeTick> data { get; set; } = Enumerable.Empty<FinnhubTradeTick>();
}

/// <summary>
/// Individual trade tick information.
/// </summary>
public class FinnhubTradeTick
{
    /// <summary>
    /// Trade price.
    /// </summary>
    public decimal p { get; set; }

    /// <summary>
    /// Trade volume.
    /// </summary>
    public decimal v { get; set; }

    /// <summary>
    /// Unix timestamp of the trade.
    /// </summary>
    public long t { get; set; }

    /// <summary>
    /// Optional trade conditions.
    /// </summary>
    public IEnumerable<string>? c { get; set; }
}
