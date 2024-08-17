namespace Domain.Interfaces;

public interface IExchangeApiClient
{
    Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency);
}
