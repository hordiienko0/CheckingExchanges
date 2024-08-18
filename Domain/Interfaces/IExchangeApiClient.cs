using Domain.Entities;

namespace Domain.Interfaces;

public interface IExchangeApiClient
{
    string ExchangeName { get; }
    Task<Result<List<ExchangeRate>>> GetRateAsync(string baseCurrency, string quoteCurrency);
}
