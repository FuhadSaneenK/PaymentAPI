using FluentValidation;
using PaymentAPI.Application.Commands.Merchants;

namespace PaymentAPI.Application.Validators.Merchants
{
    public class CreateMerchantValidator: AbstractValidator<CreateMerchantCommand>
    {
        public CreateMerchantValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Merchant name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
