using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ExchangeService : IExchangeService
{
    private readonly IEnumerable<IExchangeApiClient> _exchangeClients;

    public ExchangeService(IEnumerable<IExchangeApiClient> exchangeClients)
    {
        _exchangeClients = exchangeClients;
    }

    public async Task<ExchangeRate> GetBestRateAsync(decimal inputAmount, string inputCurrency, string outputCurrency)
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

        return new ExchangeRate { ExchangeName = bestExchange, Rate = bestOutputAmount };
    }

    public async Task<IEnumerable<ExchangeRate>> GetRatesAsync(string baseCurrency, string quoteCurrency)
    {
        var rates = new List<ExchangeRate>();

        foreach (var client in _exchangeClients)
        {
            var rate = await client.GetRateAsync(baseCurrency, quoteCurrency);
            rates.Add(new ExchangeRate { ExchangeName = client.GetType().Name.Replace("ApiClient", ""), Rate = rate });
        }

        return rates;
    }
}
