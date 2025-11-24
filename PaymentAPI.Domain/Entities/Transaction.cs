namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents a financial transaction (payment or refund) in the system.
/// </summary>
/// <remarks>
/// Transactions can be either payments (increasing account balance) or refunds (decreasing account balance).
/// Each transaction must have a unique reference number and is associated with an account and payment method.
/// </remarks>
public class Transaction
{
    /// <summary>
    /// Gets or sets the unique identifier for the transaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount of the transaction.
    /// </summary>
    /// <remarks>
    /// Must be greater than zero. For refunds, cannot exceed the original payment amount.
    /// </remarks>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was created.
    /// </summary>
    /// <remarks>
    /// Defaults to UTC current time.
    /// </remarks>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the type of transaction.
    /// </summary>
    /// <remarks>
    /// Valid values: "Payment" or "Refund".
    /// </remarks>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the transaction.
    /// </summary>
    /// <remarks>
    /// Valid values: "Pending", "Completed", or "Failed".
    /// </remarks>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique reference number for the transaction.
    /// </summary>
    /// <remarks>
    /// Must be unique across all transactions. For refunds, the format is "{OriginalReference}-REF".
    /// </remarks>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the account associated with this transaction.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the account associated with this transaction.
    /// </summary>
    /// <remarks>
    /// Navigation property.
    /// </remarks>
    public Account Account { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the payment method used for this transaction.
    /// </summary>
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the payment method used for this transaction.
    /// </summary>
    /// <remarks>
    /// Navigation property.
    /// </remarks>
    public PaymentMethod PaymentMethod { get; set; }
}
