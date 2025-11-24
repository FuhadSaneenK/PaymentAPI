using Moq;
using PaymentAPI.Application.Abstractions.Repositories;

namespace PaymentAPI.Tests.Mocks;

public static class UserRepositoryMock
{
    public static Mock<IUserRepository> Get()
    {
        return new Mock<IUserRepository>();
    }
}
