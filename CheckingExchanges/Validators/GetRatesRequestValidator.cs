using CheckingExchanges.Models;
using FluentValidation;

namespace CheckingExchanges.Validators
{
    public class GetRatesRequestValidator : AbstractValidator<GetRatesRequest>
    {
        public GetRatesRequestValidator()
        {
            RuleFor(x => x.BaseCurrency).NotEmpty().WithMessage("Base currency is required.");
            RuleFor(x => x.QuoteCurrency).NotEmpty().WithMessage("Quote currency is required.");
        }
    }
}
