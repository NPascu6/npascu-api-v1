namespace Domain.Services;

/// <summary>
/// Provides helpers for symbol normalization.
/// </summary>
public static class SymbolNormalizer
{
    /// <summary>
    /// Normalize a user supplied symbol to uppercase and trims whitespace.
    /// </summary>
    public static string Normalize(string raw)
        => string.IsNullOrWhiteSpace(raw) ? string.Empty : raw.Trim().ToUpperInvariant();

    /// <summary>
    /// Validates a normalized symbol. Expected format is EXCHANGE:TICKER (e.g., NASDAQ:AAPL).
    /// </summary>
    public static bool IsValid(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol)) return false;
        // Basic format check: EXCHANGE:TICKER with uppercase letters/numbers
        return System.Text.RegularExpressions.Regex.IsMatch(symbol, "^[A-Z]+:[A-Z0-9]+$");
    }
}
