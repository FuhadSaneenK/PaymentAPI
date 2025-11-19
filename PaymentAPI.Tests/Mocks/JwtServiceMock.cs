using Moq;
using PaymentAPI.Application.Abstractions.Services;

namespace PaymentAPI.Tests.Mocks
{
    public static class JwtServiceMock
    {
        public static Mock<IJwtService> Get()
        {
            var mock = new Mock<IJwtService>();

            // Default behavior: Generate a fake JWT token
            mock.Setup(x => x.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((int userId, string username, string role) => 
                    $"fake.jwt.token.{userId}.{username}.{role}");

            return mock;
        }
    }
}
