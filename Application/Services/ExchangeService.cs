using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;


public class ExchangeService : IExchangeService
{
    private readonly IEnumerable<IExchangeApiClient> _exchangeApiClients;

    public ExchangeService(IEnumerable<IExchangeApiClient> exchangeApiClients)
    {
        _exchangeApiClients = exchangeApiClients;
    }

    public async Task<ExchangeRate> GetBestRateAsync(decimal inputAmount, string inputCurrency, string outputCurrency)
    {
        var tasks = _exchangeApiClients.Select(async client =>
        {
            var rate = await client.GetRateAsync(inputCurrency, outputCurrency);
            return new ExchangeRate { ExchangeName = client.ExchangeName, Rate = rate };
        });

        var rates = await Task.WhenAll(tasks);
        return rates.OrderByDescending(r => r.Rate).FirstOrDefault();
    }

    public async Task<IEnumerable<ExchangeRate>> GetRatesAsync(string baseCurrency, string quoteCurrency)
    {
        var tasks = _exchangeApiClients.Select(async client =>
        {
            var rate = await client.GetRateAsync(baseCurrency, quoteCurrency);
            return new ExchangeRate { ExchangeName = client.ExchangeName, Rate = rate };
        });

        return await Task.WhenAll(tasks);
    }
}