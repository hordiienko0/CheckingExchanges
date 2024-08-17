using Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace Infrastructure.External;

public class BinanceApiClient : IExchangeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BinanceApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.binance.com");
        var response = await client.GetAsync($"/api/v3/ticker/price?symbol={baseCurrency.ToUpper()}{quoteCurrency.ToUpper()}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            return json["price"].Value<decimal>();
        }
        throw new Exception("Failed to retrieve rate from Binance API");
    }
}
