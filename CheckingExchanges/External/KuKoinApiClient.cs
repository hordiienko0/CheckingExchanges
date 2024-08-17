using CheckingExchanges.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CheckingExchanges.External
{
    public class KuCoinApiClient : IExchangeApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KuCoinApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.kucoin.com");
            var response = await client.GetAsync($"/api/v1/market/orderbook/level1?symbol={baseCurrency}-{quoteCurrency}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(content);
                return json["price"].Value<decimal>();
            }
            return 0;
        }
    }
}
