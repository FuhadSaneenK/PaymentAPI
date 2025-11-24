namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents an account within the payment system.
/// </summary>
/// <remarks>
/// An account belongs to a merchant and holds a balance that is updated by transactions (payments and refunds).
/// Each account can have multiple transactions.
/// </remarks>
public class Account
{
    /// <summary>
    /// Gets or sets the unique identifier for the account.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    public string HolderName { get; set; }

    /// <summary>
    /// Gets or sets the current balance of the account.
    /// </summary>
    /// <remarks>
    /// The balance is automatically updated when payments (increase) or refunds (decrease) are processed.
    /// Balance cannot be negative.
    /// </remarks>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the merchant that owns this account.
    /// </summary>
    public int MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the merchant that owns this account.
    /// </summary>
    /// <remarks>
    /// Navigation property.
    /// </remarks>
    public Merchant Merchant { get; set; }

    /// <summary>
    /// Gets or sets the collection of transactions associated with this account.
    /// </summary>
    /// <remarks>
    /// Navigation property. Includes both payment and refund transactions.
    /// </remarks>
    public List<Transaction> Transactions { get; set; } = new();
}
