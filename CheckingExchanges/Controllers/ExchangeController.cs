using CheckingExchanges.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CheckingExchanges.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IEnumerable<IExchangeApiClient> _exchangeClients;

        public ExchangeController(IEnumerable<IExchangeApiClient> exchangeClients)
        {
            _exchangeClients = exchangeClients;
        }

        [HttpGet("estimate")]
        public async Task<IActionResult> Estimate(decimal inputAmount, string inputCurrency, string outputCurrency)
        {
            var bestExchange = "";
            var bestOutputAmount = 0m;

            foreach (var client in _exchangeClients)
            {
                var rate = await client.GetRateAsync(inputCurrency, outputCurrency);
                var outputAmount = inputAmount * rate;

                if (outputAmount > bestOutputAmount)
                {
                    bestOutputAmount = outputAmount;
                    bestExchange = client.GetType().Name.Replace("ApiClient", "");
                }
            }

            return Ok(new { exchangeName = bestExchange, outputAmount = bestOutputAmount });
        }

        [HttpGet("getRates")]
        public async Task<IActionResult> GetRates(string baseCurrency, string quoteCurrency)
        {
            var rates = new List<object>();

            foreach (var client in _exchangeClients)
            {
                var rate = await client.GetRateAsync(baseCurrency, quoteCurrency);
                rates.Add(new { exchangeName = client.GetType().Name.Replace("ApiClient", ""), rate });
            }

            return Ok(rates);
        }
    }
}
