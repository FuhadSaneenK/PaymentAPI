using Moq;
using PaymentAPI.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Tests.Mocks
{
    public static class PaymentMethodRepositoryMock
    {
        public static Mock<IPaymentMethodRepository> Get()
        {
            return new Mock<IPaymentMethodRepository>();
        }
    }
}
