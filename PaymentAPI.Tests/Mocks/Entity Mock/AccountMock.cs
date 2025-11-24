using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Tests.Mocks.Entity_Mock;

public static class AccountMock
{
    public static Account GetAccount(
        int id = 1,
        int merchantId = 1,
        string name = "Mock Holder",
        decimal balance = 1000)
    {
        return new Account
        {
            Id = id,
            HolderName = name,
            Balance = balance,
            MerchantId = merchantId
        };
    }
}
