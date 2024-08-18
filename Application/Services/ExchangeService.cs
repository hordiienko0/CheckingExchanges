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

    public async Task<Result<ExchangeRate>> EstimateExchangeAsync(decimal inputAmount, string inputCurrency, string outputCurrency)
    {
        var tasks = _exchangeApiClients.Select(async client =>
        {
            var rateResult = await client.GetRateAsync(inputCurrency, outputCurrency);
            if (rateResult.IsSuccess)
            {
                return new ExchangeRate
                {
                    ExchangeName = client.ExchangeName,
                    Rate = rateResult.Value.First().Rate * inputAmount
                };
            }
            return null;
        });

        var rates = (await Task.WhenAll(tasks)).Where(r => r != null).ToList();
        if (rates.Any())
        {
            var bestRate = rates.OrderByDescending(r => r.Rate).FirstOrDefault();
            return Result<ExchangeRate>.Success(bestRate);
        }
        return Result<ExchangeRate>.Failure("No valid exchange rates found.");
    }

    public async Task<Result<IEnumerable<ExchangeRate>>> GetRatesAsync(string baseCurrency, string quoteCurrency)
    {
        var tasks = _exchangeApiClients.Select(async client =>
        {
            var rateResult = await client.GetRateAsync(baseCurrency, quoteCurrency);
            if (rateResult.IsSuccess)
            {
                return rateResult.Value.Select(rate => new ExchangeRate
                {
                    ExchangeName = client.ExchangeName,
                    Rate = rate.Rate
                });
            }
            return Enumerable.Empty<ExchangeRate>();
        });

        var rates = await Task.WhenAll(tasks);
        var allRates = rates.SelectMany(r => r);

        if (allRates.Any())
        {
            return Result<IEnumerable<ExchangeRate>>.Success(allRates);
        }
        return Result<IEnumerable<ExchangeRate>>.Failure("No valid exchange rates found.");
    }
}