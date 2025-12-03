using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Tests.Mocks.Entity_Mock;

public static class UserMock
{
    public static User GetUser(
        int id = 1,
        string username = "testuser",
        string passwordHash = "HASHEDPWD",
        string role = "User",
        int? merchantId = null)
    {
        return new User
        {
            Id = id,
            Username = username,
            PasswordHash = passwordHash,
            Role = role,
            MerchantId = merchantId
        };
    }
}
