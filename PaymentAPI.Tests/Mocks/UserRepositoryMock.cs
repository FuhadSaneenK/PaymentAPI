using Moq;
using PaymentAPI.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Tests.Mocks
{
    public static class UserRepositoryMock
    {
        public static Mock<IUserRepository> Get()
        {
            return new Mock<IUserRepository>();
        }
    }
}
