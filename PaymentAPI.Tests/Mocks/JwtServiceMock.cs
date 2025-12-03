using Moq;
using PaymentAPI.Application.Abstractions.Services;

namespace PaymentAPI.Tests.Mocks;

public static class JwtServiceMock
{
    public static Mock<IJwtService> Get()
    {
        var mock = new Mock<IJwtService>();

        // Default behavior: Generate a fake JWT token with optional merchantId
        mock.Setup(x => x.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
            .Returns((int userId, string username, string role, int? merchantId) => 
                $"fake.jwt.token.{userId}.{username}.{role}");

        return mock;
    }
}
