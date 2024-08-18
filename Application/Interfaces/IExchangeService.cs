using Domain.Entities;

namespace Application.Interfaces;
public interface IExchangeService
{
    Task<Result<ExchangeRate>> EstimateExchangeAsync(decimal inputAmount, string inputCurrency, string outputCurrency);
    Task<Result<IEnumerable<ExchangeRate>>> GetRatesAsync(string baseCurrency, string quoteCurrency);
}
