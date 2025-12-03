using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Application.Validators.Transactions;

namespace PaymentAPI.Tests.Validators.Transactions;

public class MakeRefundValidatorTests
{
    private readonly MakeRefundValidator _validator;

    public MakeRefundValidatorTests()
    {
        _validator = new MakeRefundValidator();
    }

    [Fact]
    public void Should_HaveError_When_Amount_IsZero()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = 0, 
            AccountId = 1, 
            ReferenceNo = "REF001",
            Reason = "Test reason"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Refund amount must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_Amount_IsNegative()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = -50, 
            AccountId = 1, 
            ReferenceNo = "REF001",
            Reason = "Test reason"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("Refund amount must be greater than zero.");
    }

    [Fact]
    public void Should_HaveError_When_AccountId_IsZero()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = 100, 
            AccountId = 0, 
            ReferenceNo = "REF001",
            Reason = "Test reason"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountId)
              .WithErrorMessage("AccountId must be valid.");
    }

    [Fact]
    public void Should_HaveError_When_ReferenceNo_IsEmpty()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = 100, 
            AccountId = 1, 
            ReferenceNo = "",
            Reason = "Test reason"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReferenceNo)
              .WithErrorMessage("Reference number is required to process refund.");
    }

    [Fact]
    public void Should_HaveError_When_Reason_IsEmpty()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = 100, 
            AccountId = 1, 
            ReferenceNo = "REF001",
            Reason = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Reason)
              .WithErrorMessage("Reason is required for refund request.");
    }

    [Fact]
    public void Should_HaveError_When_Reason_ExceedsMaxLength()
    {
        // Arrange
        var longReason = new string('A', 501); // 501 characters
        var command = new MakeRefundCommand 
        { 
            Amount = 100, 
            AccountId = 1, 
            ReferenceNo = "REF001",
            Reason = longReason
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Reason)
              .WithErrorMessage("Reason cannot exceed 500 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllFields_AreValid()
    {
        // Arrange
        var command = new MakeRefundCommand 
        { 
            Amount = 75.25m, 
            AccountId = 1, 
            ReferenceNo = "REF001",
            Reason = "Product was defective"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
