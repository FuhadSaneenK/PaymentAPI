

namespace PaymentAPI.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }= DateTime.UtcNow;
        public string Type { get; set; } = string.Empty; // e.g., Payment | Refund
        public string Status { get; set; } = string.Empty; // e.g., Pending | Completed | Failed
        public string ReferenceNumber { get; set; }=string.Empty;

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }


    }
}
