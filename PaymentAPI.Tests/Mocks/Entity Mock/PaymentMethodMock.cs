using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Tests.Mocks.Entity_Mock;

public static class PaymentMethodMock
{
    public static PaymentMethod GetPaymentMethod(
        int id = 1,
        string method = "UPI",
        string provider = "PhonePe")
    {
        return new PaymentMethod
        {
            Id = id,
            MethodName = method,
            Provider = provider
        };
    }
}
