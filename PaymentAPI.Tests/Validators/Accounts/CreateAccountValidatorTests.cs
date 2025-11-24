using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Accounts;
using PaymentAPI.Application.Validators.Accounts;

namespace PaymentAPI.Tests.Validators.Accounts;

public class CreateAccountValidatorTests
{
    private readonly CreateAccountValidator _validator;

    public CreateAccountValidatorTests()
    {
        _validator = new CreateAccountValidator();
    }

    [Fact]
    public void Should_HaveError_When_HolderName_IsEmpty()
    {
        // Arrange
        var command = new CreateAccountCommand 
        { 
            HolderName = "", 
            Balance = 100, 
            MerchantId = 1 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HolderName)
              .WithErrorMessage("Account holder name is required.");
    }

    [Fact]
    public void Should_HaveError_When_Balance_IsNegative()
    {
        // Arrange
        var command = new CreateAccountCommand 
        { 
            HolderName = "John Doe", 
            Balance = -50, 
            MerchantId = 1 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
              .WithErrorMessage("Initial balance cannot be negative.");
    }

    [Fact]
    public void Should_HaveError_When_MerchantId_IsZero()
    {
        // Arrange
        var command = new CreateAccountCommand 
        { 
            HolderName = "John Doe", 
            Balance = 100, 
            MerchantId = 0 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MerchantId)
              .WithErrorMessage("MerchantId must be valid.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllFields_AreValid()
    {
        // Arrange
        var command = new CreateAccountCommand 
        { 
            HolderName = "John Doe", 
            Balance = 1000, 
            MerchantId = 1 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_Balance_IsZero()
    {
        // Arrange
        var command = new CreateAccountCommand 
        { 
            HolderName = "John Doe", 
            Balance = 0, 
            MerchantId = 1 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
