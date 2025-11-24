namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents a payment method available in the system.
/// </summary>
/// <remarks>
/// Payment methods define how transactions are processed (e.g., Credit Card, PayPal, Bank Transfer).
/// Each payment method can be associated with multiple transactions.
/// </remarks>
public class PaymentMethod
{
    /// <summary>
    /// Gets or sets the unique identifier for the payment method.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the payment method.
    /// </summary>
    /// <remarks>
    /// Examples: "Credit Card", "Debit Card", "PayPal", "Bank Transfer", "UPI", "Wallet".
    /// </remarks>
    public string MethodName { get; set; }

    /// <summary>
    /// Gets or sets the provider or network of the payment method.
    /// </summary>
    /// <remarks>
    /// Examples: "Visa", "MasterCard", "Stripe", "Google Pay", "Paytm".
    /// </remarks>
    public string Provider { get; set; }

    /// <summary>
    /// Gets or sets the collection of transactions processed using this payment method.
    /// </summary>
    /// <remarks>
    /// Navigation property.
    /// </remarks>
    public List<Transaction> Transactions { get; set; } = new();
}
