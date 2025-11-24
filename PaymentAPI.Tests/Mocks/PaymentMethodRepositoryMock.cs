using Moq;
using PaymentAPI.Application.Abstractions.Repositories;

namespace PaymentAPI.Tests.Mocks;

public static class PaymentMethodRepositoryMock
{
    public static Mock<IPaymentMethodRepository> Get()
    {
        return new Mock<IPaymentMethodRepository>();
    }
}
