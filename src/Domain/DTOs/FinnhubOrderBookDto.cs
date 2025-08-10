namespace Domain.DTOs;

/// <summary>
/// Represents a level 2 order book snapshot returned by Finnhub.
/// </summary>
public class FinnhubOrderBookDto
{
    /// <summary>
    /// Bids in the book expressed as [price, size].
    /// </summary>
    public decimal[][] b { get; set; } = Array.Empty<decimal[]>();

    /// <summary>
    /// Asks in the book expressed as [price, size].
    /// </summary>
    public decimal[][] a { get; set; } = Array.Empty<decimal[]>();

    /// <summary>
    /// Unix timestamp of the snapshot.
    /// </summary>
    public long t { get; set; }

    /// <summary>
    /// Symbol of the book.
    /// </summary>
    public string s { get; set; } = string.Empty;
}
