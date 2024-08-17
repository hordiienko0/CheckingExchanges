using CheckingExchanges.Models;
using FluentValidation;

namespace CheckingExchanges.Validators
{
    public class EstimateRequestValidator : AbstractValidator<EstimateRequest>
    {
        public EstimateRequestValidator()
        {
            RuleFor(x => x.InputAmount).GreaterThan(0).WithMessage("Input amount must be greater than zero.");
            RuleFor(x => x.InputCurrency).NotEmpty().WithMessage("Input currency is required.");
            RuleFor(x => x.OutputCurrency).NotEmpty().WithMessage("Output currency is required.");
        }
    }
}
