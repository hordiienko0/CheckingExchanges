using Domain.Entities;
using Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace Infrastructure.External
{
    public class BinanceApiClient : IExchangeApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ExchangeName => "Binance";

        public BinanceApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<List<ExchangeRate>>> GetRateAsync(string baseCurrency, string quoteCurrency)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.binance.com");

            var sellRate = await FetchAverageRateAsync(client, baseCurrency, quoteCurrency);
            if (sellRate.IsSuccess)
            {
                return Result<List<ExchangeRate>>.Success(new List<ExchangeRate>
                {
                    new ExchangeRate
                    {
                        ExchangeName = ExchangeName,
                        Rate = sellRate.Value.Rate
                    }
                });
            }

            var buyRate = await FetchAverageRateAsync(client, quoteCurrency, baseCurrency);
            if (buyRate.IsSuccess)
            {
                return Result<List<ExchangeRate>>.Success(new List<ExchangeRate>
                {
                    new ExchangeRate
                    {
                        ExchangeName = ExchangeName,
                        Rate = Decimal.One / buyRate.Value.Rate
                    }
                });
            }

            return Result<List<ExchangeRate>>.Failure("Failed to retrieve exchange rate.");
        }

        private async Task<Result<ExchangeRate>> FetchAverageRateAsync(HttpClient client, string baseCurrency, string quoteCurrency)
        {
            var response = await client.GetAsync($"/api/v3/depth?symbol={baseCurrency.ToUpper()}{quoteCurrency.ToUpper()}&limit=1");
            if (response.IsSuccessStatusCode)
            {
                var orderBook = JObject.Parse(await response.Content.ReadAsStringAsync());
                if (orderBook["asks"] != null)
                {
                    var asks = orderBook["asks"]?.TakeLast(10).Select(a => a[0]?.Value<decimal>() ?? 0).ToList();

                    if (asks != null)
                    {
                        var averageRate = asks.Average();
                        return Result<ExchangeRate>.Success(new ExchangeRate
                        {
                            ExchangeName = ExchangeName,
                            Rate = averageRate
                        });
                    }
                }
            }
            return Result<ExchangeRate>.Failure("Failed to fetch average rate.");
        }
    }
}