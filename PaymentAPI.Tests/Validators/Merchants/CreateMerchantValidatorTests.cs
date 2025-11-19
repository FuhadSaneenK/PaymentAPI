using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Merchants;
using PaymentAPI.Application.Validators.Merchants;

namespace PaymentAPI.Tests.Validators.Merchants
{
    public class CreateMerchantValidatorTests
    {
        private readonly CreateMerchantValidator _validator;

        public CreateMerchantValidatorTests()
        {
            _validator = new CreateMerchantValidator();
        }

        [Fact]
        public void Should_HaveError_When_Name_IsEmpty()
        {
            // Arrange
            var command = new CreateMerchantCommand { Name = "", Email = "test@example.com" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Merchant name is required.");
        }

        [Fact]
        public void Should_HaveError_When_Email_IsEmpty()
        {
            // Arrange
            var command = new CreateMerchantCommand { Name = "Test Merchant", Email = "" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void Should_HaveError_When_Email_IsInvalidFormat()
        {
            // Arrange
            var command = new CreateMerchantCommand { Name = "Test Merchant", Email = "invalid-email" };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void Should_NotHaveError_When_AllFields_AreValid()
        {
            // Arrange
            var command = new CreateMerchantCommand 
            { 
                Name = "Test Merchant", 
                Email = "test@example.com" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
