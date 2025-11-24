namespace PaymentAPI.Application.DTOs;

/// <summary>
/// Data Transfer Object representing an account.
/// </summary>
/// <remarks>
/// Used for transferring account data between layers without exposing domain entities.
/// </remarks>
/// <example>
/// <code>
/// {
///   "id": 1,
///   "holderName": "John Doe",
///   "balance": 1500.50,
///   "merchantId": 5
/// }
/// </code>
/// </example>
public class AccountDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the account.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    /// <example>John Doe</example>
    public string HolderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current balance of the account.
    /// </summary>
    /// <remarks>
    /// Balance is updated automatically by payment and refund transactions.
    /// </remarks>
    /// <example>1500.50</example>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the merchant that owns this account.
    /// </summary>
    /// <example>5</example>
    public int MerchantId { get; set; }
}
