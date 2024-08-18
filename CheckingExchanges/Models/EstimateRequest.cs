namespace CheckingExchanges.Models;

public class EstimateRequest
{
    public decimal InputAmount { get; set; }
    public string InputCurrency { get; set; }
    public string OutputCurrency { get; set; }
}
