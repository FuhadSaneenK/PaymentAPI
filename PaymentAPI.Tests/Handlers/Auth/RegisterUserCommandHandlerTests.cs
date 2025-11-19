using Moq;
using PaymentAPI.Application.Handlers.Auth;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Auth
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly CancellationToken _ct = CancellationToken.None;

        [Fact]
        public async Task Should_ReturnFail_When_Username_AlreadyExists()
        {
            // Arrange
            var userRepo = UserRepositoryMock.Get();
            var passwordHasher = PasswordHasherMock.Get();

            var existingUser = UserMock.GetUser(1, "existinguser", "HASHED_password", "User");

            userRepo.Setup(x => x.GetByUsernameAsync("existinguser", _ct))
                    .ReturnsAsync(existingUser);

            var handler = new RegisterUserCommandHandler(
                userRepo.Object,
                passwordHasher.Object);

            // Act
            var result = await handler.Handle(
                new RegisterUserCommand { Username = "existinguser", Password = "password123" }, 
                _ct);

            // Assert
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Username already exists");
        }

        [Fact]
        public async Task Should_CreateUser_When_Username_IsAvailable()
        {
            // Arrange
            var userRepo = UserRepositoryMock.Get();
            var passwordHasher = PasswordHasherMock.Get();

            userRepo.Setup(x => x.GetByUsernameAsync("newuser", _ct))
                    .ReturnsAsync((User)null);

            userRepo.Setup(x => x.AddAsync(It.IsAny<User>(), _ct))
                    .Returns(Task.CompletedTask);

            userRepo.Setup(x => x.SaveChangesAsync(_ct))
                    .ReturnsAsync(1);

            var handler = new RegisterUserCommandHandler(
                userRepo.Object,
                passwordHasher.Object);

            // Act
            var result = await handler.Handle(
                new RegisterUserCommand { Username = "newuser", Password = "password123" }, 
                _ct);

            // Assert
            result.Status.ShouldBe(200);
            result.Data.ShouldBe("User registered successfully");
            
            // Verify repository methods were called
            userRepo.Verify(x => x.AddAsync(It.Is<User>(u => 
                u.Username == "newuser" && 
                u.PasswordHash == "HASHED_password123"), _ct), Times.Once);
            userRepo.Verify(x => x.SaveChangesAsync(_ct), Times.Once);
        }
    }
}
