using System.Net.Http.Json;
using Domain.DTOs;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Infrastructure.Clients;

/// <summary>
/// Lightweight HTTP client for Finnhub's REST API.
/// </summary>
public class FinnhubClient : IFinnhubClient
{
    private readonly HttpClient _httpClient;
    private readonly FinnhubOptions _options;

    public FinnhubClient(HttpClient httpClient, IOptions<FinnhubOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.RestBaseUrl);
    }

    public async Task<FinnhubOrderBookDto?> GetOrderBookAsync(string symbol, int? depth = null, CancellationToken ct = default)
    {
        var url = $"orderbook?symbol={symbol}&token={_options.ApiKey}";
        if (depth.HasValue) url += $"&depth={depth.Value}";
        return await _httpClient.GetFromJsonAsync<FinnhubOrderBookDto>(url, ct);
    }

    public async Task<FinnhubTradeDto?> GetTradesAsync(string symbol, DateTime from, DateTime to, int? limit = null, CancellationToken ct = default)
    {
        var url = $"stock/trades?symbol={symbol}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&token={_options.ApiKey}";
        if (limit.HasValue) url += $"&limit={limit.Value}";
        return await _httpClient.GetFromJsonAsync<FinnhubTradeDto>(url, ct);
    }

    public Task<FinnhubQuoteDto?> GetQuoteAsync(string symbol, CancellationToken ct = default)
    {
        var url = $"quote?symbol={symbol}&token={_options.ApiKey}";
        return _httpClient.GetFromJsonAsync<FinnhubQuoteDto>(url, ct);
    }

    public Task<FinnhubCandleDto?> GetCandlesAsync(string symbol, string resolution, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var url = $"stock/candle?symbol={symbol}&resolution={resolution}&from={new DateTimeOffset(from).ToUnixTimeSeconds()}&to={new DateTimeOffset(to).ToUnixTimeSeconds()}&token={_options.ApiKey}";
        return _httpClient.GetFromJsonAsync<FinnhubCandleDto>(url, ct);
    }

    public Task<FinnhubCompanyProfileDto?> GetProfileAsync(string symbol, CancellationToken ct = default)
    {
        var url = $"stock/profile2?symbol={symbol}&token={_options.ApiKey}";
        return _httpClient.GetFromJsonAsync<FinnhubCompanyProfileDto>(url, ct);
    }

    public async Task<IEnumerable<FinnhubSymbolDto>> GetSymbolsAsync(string exchange, CancellationToken ct = default)
    {
        var url = $"stock/symbol?exchange={exchange}&token={_options.ApiKey}";
        return await _httpClient.GetFromJsonAsync<IEnumerable<FinnhubSymbolDto>>(url, ct) 
               ?? Enumerable.Empty<FinnhubSymbolDto>();
    }
}
