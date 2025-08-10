namespace Api.DTOs;

public class FinnhubQuoteDto
{
    /// <summary>
    /// Current price.
    /// </summary>
    public decimal c { get; set; }

    /// <summary>
    /// High price of the day.
    /// </summary>
    public decimal h { get; set; }

    /// <summary>
    /// Low price of the day.
    /// </summary>
    public decimal l { get; set; }

    /// <summary>
    /// Open price of the day.
    /// </summary>
    public decimal o { get; set; }

    /// <summary>
    /// Previous close price.
    /// </summary>
    public decimal pc { get; set; }

    /// <summary>
    /// Unix timestamp when the quote was generated.
    /// </summary>
    public long t { get; set; }
}
