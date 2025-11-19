using FluentValidation.TestHelper;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Validators.Auth;

namespace PaymentAPI.Tests.Validators.Auth
{
    public class RegisterUserValidatorTests
    {
        private readonly RegisterUserValidator _validator;

        public RegisterUserValidatorTests()
        {
            _validator = new RegisterUserValidator();
        }

        [Fact]
        public void Should_HaveError_When_Username_IsEmpty()
        {
            // Arrange
            var command = new RegisterUserCommand 
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
        public void Should_HaveError_When_Username_IsTooShort()
        {
            // Arrange
            var command = new RegisterUserCommand 
            { 
                Username = "ab", 
                Password = "password123" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage("Username must be at least 3 characters long.");
        }

        [Fact]
        public void Should_HaveError_When_Password_IsEmpty()
        {
            // Arrange
            var command = new RegisterUserCommand 
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
        public void Should_HaveError_When_Password_IsTooShort()
        {
            // Arrange
            var command = new RegisterUserCommand 
            { 
                Username = "testuser", 
                Password = "12345" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 6 characters long.");
        }

        [Fact]
        public void Should_HaveError_When_Both_Username_And_Password_AreEmpty()
        {
            // Arrange
            var command = new RegisterUserCommand 
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
            var command = new RegisterUserCommand 
            { 
                Username = "testuser", 
                Password = "password123" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_NotHaveError_When_Username_IsExactlyMinimumLength()
        {
            // Arrange
            var command = new RegisterUserCommand 
            { 
                Username = "abc", 
                Password = "password123" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Should_NotHaveError_When_Password_IsExactlyMinimumLength()
        {
            // Arrange
            var command = new RegisterUserCommand 
            { 
                Username = "testuser", 
                Password = "123456" 
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
