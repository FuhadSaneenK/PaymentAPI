using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Tests.Mocks.Entity_Mock;

public static class MerchantMock
{
    public static Merchant GetMerchant(
        int id = 1,
        string name = "Mock Merchant",
        string email = "merchant@mail.com")
    {
        return new Merchant
        {
            Id = id,
            Name = name,
            Email = email,
            Accounts = new List<Account>()
        };
    }
}
