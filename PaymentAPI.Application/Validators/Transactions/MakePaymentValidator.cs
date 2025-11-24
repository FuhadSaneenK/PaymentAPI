using FluentValidation;
using PaymentAPI.Application.Commands.Transactions;

namespace PaymentAPI.Application.Validators.Transactions
{
    /// <summary>
    /// Validator for <see cref="MakePaymentCommand"/> ensuring payment data is valid before processing.
    /// </summary>
    /// <remarks>
    /// Validates amount (must be positive), account ID, payment method ID, and reference number presence.
    /// Additional business rules (account existence, reference uniqueness) are validated in the handler.
    /// </remarks>
    public class MakePaymentValidator : AbstractValidator<MakePaymentCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MakePaymentValidator"/> class with validation rules.
        /// </summary>
        public MakePaymentValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0)
                .WithMessage("AccountId must be valid.");

            RuleFor(x => x.PaymentMethodId)
                .GreaterThan(0)
                .WithMessage("PaymentMethodId must be valid.");

            RuleFor(x => x.ReferenceNo)
                .NotEmpty()
                .WithMessage("Reference number is required.");
        }
    }
}
