using Moq;
using PaymentAPI.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Tests.Mocks
{
    public static class MerchantRepositoryMock
    {
        public static Mock<IMerchantRepository> Get()
        {
            return new Mock<IMerchantRepository>();
        }
    }
}
