using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Validators.Auth;

namespace PaymentAPI.Tests.Validators.Auth
{
    public class LoginUserValidatorTests
    {
        private readonly LoginUserValidator _validator;

        public LoginUserValidatorTests()
        {
            _validator = new LoginUserValidator();
        }

        [Fact]
        public void Should_HaveError_When_Username_IsEmpty()
        {
            // Arrange
            var command = new LoginUserCommand 
            { 
                Username = "", 
                Password = "password123" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage("Username is required.");
        }

        [Fact]
        public void Should_HaveError_When_Password_IsEmpty()
        {
            // Arrange
            var command = new LoginUserCommand 
            { 
                Username = "testuser", 
                Password = "" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void Should_HaveError_When_Both_Username_And_Password_AreEmpty()
        {
            // Arrange
            var command = new LoginUserCommand 
            { 
                Username = "", 
                Password = "" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_NotHaveError_When_AllFields_AreValid()
        {
            // Arrange
            var command = new LoginUserCommand 
            { 
                Username = "testuser", 
                Password = "password123" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
