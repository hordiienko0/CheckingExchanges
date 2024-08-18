using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Infrastructure.External;

public class KuCoinApiClient : IExchangeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KuCoinApiClient> _logger;

    public string ExchangeName => "KuCoin";


    public KuCoinApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<KuCoinApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<List<ExchangeRate>>> GetRateAsync(string baseCurrency, string quoteCurrency)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.kucoin.com");

        // Fetch sell rate
        var sellRateResult = await FetchAverageRateAsync(client, baseCurrency, quoteCurrency);
        if (sellRateResult.IsSuccess)
        {
            return Result<List<ExchangeRate>>.Success(new List<ExchangeRate>
            {
                new ExchangeRate
                {
                    ExchangeName = ExchangeName,
                    Rate = sellRateResult.Value.Rate
                }
            });
        }

        // Fetch buy rate
        var buyRateResult = await FetchAverageRateAsync(client, quoteCurrency, baseCurrency);
        if (buyRateResult.IsSuccess)
        {
            return Result<List<ExchangeRate>>.Success(new List<ExchangeRate>
            {
                new ExchangeRate
                {
                    ExchangeName = ExchangeName,
                    Rate = Decimal.One / buyRateResult.Value.Rate
                }
            });
        }

        _logger.LogWarning($"Failed to retrieve exchange rate from {ExchangeName}-apiClient.");
        return Result<List<ExchangeRate>>.Failure($"Failed to retrieve exchange rate from {ExchangeName}-apiClient.");
    }

    private async Task<Result<ExchangeRate>> FetchAverageRateAsync(HttpClient client, string baseCurrency, string quoteCurrency)
    {
        var response = await client.GetAsync($"/api/v1/market/orderbook/level2_20?symbol={baseCurrency.ToUpper()}-{quoteCurrency.ToUpper()}");
        if (response.IsSuccessStatusCode)
        {
            var orderBook = JObject.Parse(await response.Content.ReadAsStringAsync());
            var asks = orderBook["data"]?["asks"]?.TakeLast(10).Select(a => a[0]?.Value<decimal>() ?? 0).ToList();

            if (asks != null && asks.Any())
            {
                var averageRate = asks.Average();
                return Result<ExchangeRate>.Success(new ExchangeRate
                {
                    ExchangeName = ExchangeName,
                    Rate = averageRate
                });
            }
        }
        _logger.LogWarning($"Failed to fetch average rate from {ExchangeName}-apiClient.");
        return Result<ExchangeRate>.Failure($"Failed to fetch average rate from {ExchangeName}-apiClient.");
    }
}