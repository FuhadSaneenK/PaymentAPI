using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Tests.Mocks.Entity_Mock
{
    public static class UserMock
    {
        public static User GetUser(
            int id = 1,
            string username = "testuser",
            string passwordHash = "HASHEDPWD",
            string role = "User")
        {
            return new User
            {
                Id = id,
                Username = username,
                PasswordHash = passwordHash,
                Role = role
            };
        }
    }
}
