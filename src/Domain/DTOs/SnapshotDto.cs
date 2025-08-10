namespace Domain.DTOs;

/// <summary>
/// Lightweight quote snapshot.
/// </summary>
public class SnapshotDto
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Last { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? PrevClose { get; set; }
    public long Ts { get; set; }
}
