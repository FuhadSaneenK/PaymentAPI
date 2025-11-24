using FluentValidation;
using PaymentAPI.Application.Commands.Auth;

namespace PaymentAPI.Application.Validators.Auth
{
    /// <summary>
    /// Validator for <see cref="RegisterUserCommand"/> ensuring user registration data meets requirements.
    /// </summary>
    /// <remarks>
    /// Validates username (minimum 3 characters) and password (minimum 6 characters) requirements.
    /// Username uniqueness is validated in the handler.
    /// </remarks>
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserValidator"/> class with validation rules.
        /// </summary>
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters long.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");
        }
    }
}
