using Moq;
using PaymentAPI.Application.Abstractions.Repositories;

namespace PaymentAPI.Tests.Mocks;

public static class TransactionRepositoryMock
{
    public static Mock<ITransactionRepository> Get()
    {
        return new Mock<ITransactionRepository>();
    }
}
