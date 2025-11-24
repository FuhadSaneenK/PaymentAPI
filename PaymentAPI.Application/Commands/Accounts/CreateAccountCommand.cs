using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;


namespace PaymentAPI.Application.Commands.Accounts;

/// <summary>
/// Command to create a new account for a specific merchant.
/// Validates merchant existence before creating the account.
/// </summary>
/// <example>
/// <code>
/// {
///   "holderName": "John Doe Savings",
///   "balance": 5000.00,
///   "merchantId": 1
/// }
/// </code>
/// </example>
public class CreateAccountCommand:IRequest<ApiResponse<AccountDto>>
{
    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    /// <example>John Doe Savings</example>
    public string HolderName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the initial balance for the account.
    /// </summary>
    /// <example>5000.00</example>
    public decimal Balance { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the merchant who owns this account.
    /// </summary>
    /// <example>1</example>
    public int MerchantId { get; set; }
}
