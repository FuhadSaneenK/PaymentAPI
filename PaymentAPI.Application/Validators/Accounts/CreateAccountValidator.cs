using FluentValidation;
using PaymentAPI.Application.Commands.Accounts;

namespace PaymentAPI.Application.Validators.Accounts
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.HolderName)
                .NotEmpty()
                .WithMessage("Account holder name is required.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Initial balance cannot be negative.");

            RuleFor(x => x.MerchantId)
                .GreaterThan(0)
                .WithMessage("MerchantId must be valid.");
        }
    }
}
