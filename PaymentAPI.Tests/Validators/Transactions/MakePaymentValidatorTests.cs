using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Application.Validators.Transactions;

namespace PaymentAPI.Tests.Validators.Transactions;

public class MakePaymentValidatorTests
{
    private readonly MakePaymentValidator _validator;

    public MakePaymentValidatorTests()
    {
        _validator = new MakePaymentValidator();
    }

    [Fact]
    public void Should_HaveError_When_Amount_IsZero()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = 0, 
            AccountId = 1, 
            PaymentMethodId = 1, 
            ReferenceNo = "REF001" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_Amount_IsNegative()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = -100, 
            AccountId = 1, 
            PaymentMethodId = 1, 
            ReferenceNo = "REF001" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_AccountId_IsZero()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = 100, 
            AccountId = 0, 
            PaymentMethodId = 1, 
            ReferenceNo = "REF001" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountId)
              .WithErrorMessage("AccountId must be valid.");
    }

    [Fact]
    public void Should_HaveError_When_PaymentMethodId_IsZero()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = 100, 
            AccountId = 1, 
            PaymentMethodId = 0, 
            ReferenceNo = "REF001" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId)
              .WithErrorMessage("PaymentMethodId must be valid.");
    }

    [Fact]
    public void Should_HaveError_When_ReferenceNo_IsEmpty()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = 100, 
            AccountId = 1, 
            PaymentMethodId = 1, 
            ReferenceNo = "" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReferenceNo)
              .WithErrorMessage("Reference number is required.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllFields_AreValid()
    {
        // Arrange
        var command = new MakePaymentCommand 
        { 
            Amount = 100.50m, 
            AccountId = 1, 
            PaymentMethodId = 1, 
            ReferenceNo = "REF001" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
