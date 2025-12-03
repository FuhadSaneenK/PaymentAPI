using FluentValidation;
using PaymentAPI.Application.Commands.Transactions;

namespace PaymentAPI.Application.Validators.Transactions
{
    public class MakeRefundValidator : AbstractValidator<MakeRefundCommand>
    {
        public MakeRefundValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Refund amount must be greater than zero.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0)
                .WithMessage("AccountId must be valid.");

            RuleFor(x => x.ReferenceNo)
                .NotEmpty()
                .WithMessage("Reference number is required to process refund.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason is required for refund request.")
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
