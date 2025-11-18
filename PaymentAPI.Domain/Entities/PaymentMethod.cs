

namespace PaymentAPI.Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string MethodName { get; set; } // e.g., Credit Card, PayPal, Bank Transfer
        public string Provider { get; set; } // e.g., Visa, MasterCard, Stripe

        public List<Transaction> Transactions { get; set; } = new();
    }
}
