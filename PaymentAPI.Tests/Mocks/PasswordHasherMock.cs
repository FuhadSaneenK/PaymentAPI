using Moq;
using PaymentAPI.Application.Abstractions.Services;

namespace PaymentAPI.Tests.Mocks
{
    public static class PasswordHasherMock
    {
        public static Mock<IPasswordHasher> Get()
        {
            var mock = new Mock<IPasswordHasher>();

            // Default behavior: Hash returns the password prefixed with "HASHED_"
            mock.Setup(x => x.Hash(It.IsAny<string>()))
                .Returns((string password) => $"HASHED_{password}");

            // Default behavior: Verify returns true if hashed password starts with "HASHED_" 
            // and matches the original password
            mock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => hash == $"HASHED_{password}");

            return mock;
        }
    }
}
