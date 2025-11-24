using Moq;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Handlers.Auth;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Tests.Mocks;
using PaymentAPI.Tests.Mocks.Entity_Mock;
using Shouldly;

namespace PaymentAPI.Tests.Handlers.Auth;

public class LoginUserCommandHandlerTests
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task Should_ReturnFail_When_User_NotFound()
    {
        // Arrange
        var userRepo = UserRepositoryMock.Get();
        var passwordHasher = PasswordHasherMock.Get();
        var jwtService = JwtServiceMock.Get();
        var logger = LoggerMock.Create<LoginUserCommandHandler>();

        userRepo.Setup(x => x.GetByUsernameAsync("nonexistent", _ct))
                .ReturnsAsync((User)null);

        var handler = new LoginUserCommandHandler(
            userRepo.Object,
            passwordHasher.Object,
            jwtService.Object,
            logger);

        // Act
        var result = await handler.Handle(
            new LoginUserCommand { Username = "nonexistent", Password = "password" }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Invalid credentials");
    }

    [Fact]
    public async Task Should_ReturnFail_When_Password_IsInvalid()
    {
        // Arrange
        var userRepo = UserRepositoryMock.Get();
        var passwordHasher = PasswordHasherMock.Get();
        var jwtService = JwtServiceMock.Get();
        var logger = LoggerMock.Create<LoginUserCommandHandler>();

        var user = UserMock.GetUser(1, "testuser", "HASHED_correctpassword", "User");

        userRepo.Setup(x => x.GetByUsernameAsync("testuser", _ct))
                .ReturnsAsync(user);

        var handler = new LoginUserCommandHandler(
            userRepo.Object,
            passwordHasher.Object,
            jwtService.Object,
            logger);

        // Act
        var result = await handler.Handle(
            new LoginUserCommand { Username = "testuser", Password = "wrongpassword" }, 
            _ct);

        // Assert
        result.Status.ShouldBe(400);
        result.Message.ShouldBe("Invalid credentials");
    }

    [Fact]
    public async Task Should_ReturnToken_When_Credentials_AreValid()
    {
        // Arrange
        var userRepo = UserRepositoryMock.Get();
        var passwordHasher = PasswordHasherMock.Get();
        var jwtService = JwtServiceMock.Get();
        var logger = LoggerMock.Create<LoginUserCommandHandler>();

        var user = UserMock.GetUser(1, "testuser", "HASHED_correctpassword", "User");

        userRepo.Setup(x => x.GetByUsernameAsync("testuser", _ct))
                .ReturnsAsync(user);

        var handler = new LoginUserCommandHandler(
            userRepo.Object,
            passwordHasher.Object,
            jwtService.Object,
            logger);

        // Act
        var result = await handler.Handle(
            new LoginUserCommand { Username = "testuser", Password = "correctpassword" }, 
            _ct);

        // Assert
        result.Status.ShouldBe(200);
        result.Message.ShouldBe("Login successful");
        result.Data.ShouldNotBeNullOrEmpty();
        result.Data.ShouldStartWith("fake.jwt.token");
    }
}
