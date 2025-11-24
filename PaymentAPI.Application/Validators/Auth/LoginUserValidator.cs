using FluentValidation;
using PaymentAPI.Application.Commands.Auth;

namespace PaymentAPI.Application.Validators.Auth
{
    /// <summary>
    /// Validator for <see cref="LoginUserCommand"/> ensuring login credentials are provided.
    /// </summary>
    /// <remarks>
    /// Validates that both username and password are not empty.
    /// Actual credential verification is performed in the handler.
    /// </remarks>
    public class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserValidator"/> class with validation rules.
        /// </summary>
        public LoginUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
