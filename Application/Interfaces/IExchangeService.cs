using Domain.Entities;

namespace Application.Interfaces;
public interface IExchangeService
{
    Task<ExchangeRate> GetBestRateAsync(decimal inputAmount, string inputCurrency, string outputCurrency);
    Task<IEnumerable<ExchangeRate>> GetRatesAsync(string baseCurrency, string quoteCurrency);
}
