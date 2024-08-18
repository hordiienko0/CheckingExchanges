namespace Domain.Interfaces;

public interface IExchangeApiClient
{
    string ExchangeName { get; }
    Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency);
}
