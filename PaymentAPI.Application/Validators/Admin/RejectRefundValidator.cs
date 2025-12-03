using FluentValidation;
using PaymentAPI.Application.Commands.Admin;

namespace PaymentAPI.Application.Validators.Admin;

/// <summary>
/// Validator for the <see cref="RejectRefundCommand"/>.
/// </summary>
public class RejectRefundValidator : AbstractValidator<RejectRefundCommand>
{
    public RejectRefundValidator()
    {
        RuleFor(x => x.RefundRequestId)
            .GreaterThan(0)
            .WithMessage("RefundRequestId must be valid.");

        RuleFor(x => x.AdminUserId)
            .GreaterThan(0)
            .WithMessage("AdminUserId must be valid.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required for rejection.")
            .MaximumLength(1000)
            .WithMessage("Reason cannot exceed 1000 characters.");
    }
}
