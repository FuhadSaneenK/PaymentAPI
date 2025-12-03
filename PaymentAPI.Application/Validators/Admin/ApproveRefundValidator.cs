using FluentValidation;
using PaymentAPI.Application.Commands.Admin;

namespace PaymentAPI.Application.Validators.Admin;

/// <summary>
/// Validator for the <see cref="ApproveRefundCommand"/>.
/// </summary>
public class ApproveRefundValidator : AbstractValidator<ApproveRefundCommand>
{
    public ApproveRefundValidator()
    {
        RuleFor(x => x.RefundRequestId)
            .GreaterThan(0)
            .WithMessage("RefundRequestId must be valid.");

        RuleFor(x => x.AdminUserId)
            .GreaterThan(0)
            .WithMessage("AdminUserId must be valid.");

        RuleFor(x => x.Comments)
            .MaximumLength(1000)
            .WithMessage("Comments cannot exceed 1000 characters.");
    }
}
