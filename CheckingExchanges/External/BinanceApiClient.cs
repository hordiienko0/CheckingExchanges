using CheckingExchanges.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CheckingExchanges.External;

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
        //var response = await client.GetStringAsync($"/api/v3/depth?symbol={baseCurrency}{quoteCurrency}&limit=1");
        var response = await client.GetAsync($"/api/v3/depth?symbol={baseCurrency}{quoteCurrency}&limit=1");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            return json["price"].Value<decimal>();
        }
        return 0;        
    }
}
