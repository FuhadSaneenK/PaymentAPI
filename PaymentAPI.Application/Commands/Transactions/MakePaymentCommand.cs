using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Transactions;

/// <summary>
/// Command to process a payment transaction and update account balance.
/// Validates account, payment method, and reference number uniqueness.
/// </summary>
/// <example>
/// <code>
/// {
///   "amount": 500.00,
///   "accountId": 1,
///   "paymentMethodId": 2,
///   "referenceNo": "PAY-2024-12-001"
/// }
/// </code>
/// </example>
public class MakePaymentCommand:IRequest<ApiResponse<TransactionDto>>
{
    /// <summary>
    /// Gets or sets the payment amount to be credited to the account.
    /// </summary>
    /// <remarks>
    /// Must be greater than zero.
    /// </remarks>
    /// <example>500.00</example>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the account receiving the payment.
    /// </summary>
    /// <example>1</example>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the payment method used for this transaction.
    /// </summary>
    /// <remarks>
    /// Examples: 1 = Credit Card, 2 = Debit Card, 3 = UPI, etc.
    /// </remarks>
    /// <example>2</example>
    public int PaymentMethodId { get; set; }
    
    /// <summary>
    /// Gets or sets the unique reference number for this payment. Must be unique across all transactions.
    /// </summary>
    /// <remarks>
    /// Typically follows a format like PAY-YYYY-MM-NNN or similar.
    /// </remarks>
    /// <example>PAY-2024-12-001</example>
    public string ReferenceNo { get; set; } = string.Empty;
}
