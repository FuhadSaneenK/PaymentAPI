
namespace PaymentAPI.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; }

        public List<Transaction> Transactions { get; set; } = new();

    }
}
