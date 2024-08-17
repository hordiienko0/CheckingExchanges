namespace CheckingExchanges.Models
{
    public class GetRatesRequest
    {
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
    }
}
