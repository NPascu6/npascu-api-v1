using System.Net.Http.Json;
using Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Clients;

/// <summary>
/// Lightweight HTTP client for Finnhub's REST API.
/// </summary>
public class FinnhubClient : IFinnhubClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public FinnhubClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["FINNHUB_API_KEY"] ?? string.Empty;
        _httpClient.BaseAddress = new Uri("https://finnhub.io/api/v1/");
    }

    public async Task<FinnhubOrderBookDto?> GetOrderBookAsync(string symbol, int? depth = null, CancellationToken ct = default)
    {
        var url = $"orderbook?symbol={symbol}&token={_apiKey}";
        if (depth.HasValue) url += $"&depth={depth.Value}";
        return await _httpClient.GetFromJsonAsync<FinnhubOrderBookDto>(url, ct);
    }

    public async Task<FinnhubTradeDto?> GetTradesAsync(string symbol, DateTime from, DateTime to, int? limit = null, CancellationToken ct = default)
    {
        var url = $"stock/trades?symbol={symbol}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&token={_apiKey}";
        if (limit.HasValue) url += $"&limit={limit.Value}";
        return await _httpClient.GetFromJsonAsync<FinnhubTradeDto>(url, ct);
    }

    public Task<FinnhubQuoteDto?> GetQuoteAsync(string symbol, CancellationToken ct = default)
    {
        var url = $"quote?symbol={symbol}&token={_apiKey}";
        return _httpClient.GetFromJsonAsync<FinnhubQuoteDto>(url, ct);
    }
}
