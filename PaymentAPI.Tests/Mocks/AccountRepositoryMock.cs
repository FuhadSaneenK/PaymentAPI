using Moq;
using PaymentAPI.Application.Abstractions.Repositories;


namespace PaymentAPI.Tests.Mocks
{
    public static class AccountRepositoryMock
    {
        public static Mock<IAccountRepository> Get()
        {
            return new Mock<IAccountRepository>();
        }
    }
}
