namespace CheckingExchanges.Interfaces;

public interface IExchangeApiClient
{
    Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency);
}
