using Moq;
using PaymentAPI.Application.Abstractions.Repositories;

namespace PaymentAPI.Tests.Mocks;

public static class MerchantRepositoryMock
{
    public static Mock<IMerchantRepository> Get()
    {
        return new Mock<IMerchantRepository>();
    }
}
