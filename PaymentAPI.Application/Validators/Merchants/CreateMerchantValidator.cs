using FluentValidation;
using PaymentAPI.Application.Commands.Merchants;

namespace PaymentAPI.Application.Validators.Merchants
{
    /// <summary>
    /// Validator for <see cref="CreateMerchantCommand"/> ensuring merchant creation data is valid.
    /// </summary>
    /// <remarks>
    /// Validates that merchant name is provided and email is in valid format.
    /// Email uniqueness is validated in the handler.
    /// </remarks>
    public class CreateMerchantValidator: AbstractValidator<CreateMerchantCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMerchantValidator"/> class with validation rules.
        /// </summary>
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
