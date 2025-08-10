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
}
