namespace PaymentAPI.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a comprehensive merchant summary with aggregated data.
/// </summary>
/// <remarks>
/// Contains merchant details along with aggregated statistics including balance, 
/// account count, and transaction summaries across all merchant accounts.
/// </remarks>
/// <example>
/// <code>
/// {
///   "merchantId": 1,
///   "merchantName": "TechMart Electronics",
///   "email": "contact@techmart.com",
///   "totalHolders": 5,
///   "totalBalance": 25000.75,
///   "totalTransactions": 150,
///   "totalPayments": 120,
///   "totalRefunds": 30
/// }
/// </code>
/// </example>
public class MerchantSummaryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the merchant.
    /// </summary>
    /// <example>1</example>
    public int MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the business name of the merchant.
    /// </summary>
    /// <example>TechMart Electronics</example>
    public string MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the merchant.
    /// </summary>
    /// <example>contact@techmart.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of account holders (accounts) under this merchant.
    /// </summary>
    /// <example>5</example>
    public int TotalHolders { get; set; }

    /// <summary>
    /// Gets or sets the total balance across all accounts owned by this merchant.
    /// </summary>
    /// <remarks>
    /// This is the sum of balances from all accounts associated with the merchant.
    /// </remarks>
    /// <example>25000.75</example>
    public decimal TotalBalance { get; set; }

    /// <summary>
    /// Gets or sets the total count of all transactions (payments + refunds) across all merchant accounts.
    /// </summary>
    /// <example>150</example>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Gets or sets the total number of payment transactions across all merchant accounts.
    /// </summary>
    /// <example>120</example>
    public int TotalPayments { get; set; }

    /// <summary>
    /// Gets or sets the total number of refund transactions across all merchant accounts.
    /// </summary>
    /// <example>30</example>
    public int TotalRefunds { get; set; }
}
