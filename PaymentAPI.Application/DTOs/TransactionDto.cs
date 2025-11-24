namespace PaymentAPI.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a transaction (payment or refund).
/// </summary>
/// <remarks>
/// Used for transferring transaction data between layers without exposing domain entities.
/// Contains all transaction details including amount, type, status, and reference information.
/// </remarks>
/// <example>
/// Payment Transaction:
/// <code>
/// {
///   "id": 1,
///   "amount": 500.00,
///   "type": "Payment",
///   "status": "Completed",
///   "referenceNo": "TXN-2024-001",
///   "accountId": 1,
///   "paymentMethodId": 2,
///   "date": "2024-12-10T14:30:00Z"
/// }
/// </code>
/// 
/// Refund Transaction:
/// <code>
/// {
///   "id": 2,
///   "amount": 150.00,
///   "type": "Refund",
///   "status": "Completed",
///   "referenceNo": "TXN-2024-001-REF",
///   "accountId": 1,
///   "paymentMethodId": 2,
///   "date": "2024-12-11T10:15:00Z"
/// }
/// </code>
/// </example>
public class TransactionDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the transaction.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount of the transaction.
    /// </summary>
    /// <example>500.00</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the type of transaction.
    /// </summary>
    /// <remarks>
    /// Valid values: "Payment" or "Refund".
    /// </remarks>
    /// <example>Payment</example>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the transaction.
    /// </summary>
    /// <remarks>
    /// Valid values: "Pending", "Completed", or "Failed".
    /// </remarks>
    /// <example>Completed</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique reference number for the transaction.
    /// </summary>
    /// <remarks>
    /// For refunds, the format is "{OriginalPaymentReference}-REF".
    /// </remarks>
    /// <example>TXN-2024-001</example>
    public string ReferenceNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the account associated with this transaction.
    /// </summary>
    /// <example>1</example>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the payment method used for this transaction.
    /// </summary>
    /// <remarks>
    /// May be null for certain transaction types.
    /// </remarks>
    /// <example>2</example>
    public int? PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was created.
    /// </summary>
    /// <example>2024-12-10T14:30:00Z</example>
    public DateTime Date { get; set; }
}
