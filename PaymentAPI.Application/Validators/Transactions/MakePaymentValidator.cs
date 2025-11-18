using FluentValidation;
using PaymentAPI.Application.Commands.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Validators.Transactions
{
    public class MakePaymentValidator : AbstractValidator<MakePaymentCommand>
    {
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
