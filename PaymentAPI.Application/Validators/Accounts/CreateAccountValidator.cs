using FluentValidation;
using PaymentAPI.Application.Commands.Accounts;

namespace PaymentAPI.Application.Validators.Accounts;

/// <summary>
/// Validator for <see cref="CreateAccountCommand"/> ensuring account creation data is valid.
/// </summary>
/// <remarks>
/// Validates that holder name is provided, balance is non-negative, and merchant ID is valid.
/// </remarks>
public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAccountValidator"/> class with validation rules.
    /// </summary>
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
