using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Transactions;

/// <summary>
/// Command to process a refund transaction and decrease account balance.
/// Validates the original payment exists and refund amount does not exceed original payment.
/// </summary>
/// <example>
/// <code>
/// {
///   "amount": 150.00,
///   "accountId": 1,
///   "referenceNo": "PAY-2024-12-001"
/// }
/// </code>
/// This creates a refund with reference "PAY-2024-12-001-REF"
/// </example>
public class MakeRefundCommand : IRequest<ApiResponse<TransactionDto>>
{
    /// <summary>
    /// Gets or sets the refund amount to be debited from the account.
    /// Must not exceed the original payment amount.
    /// </summary>
    /// <remarks>
    /// Must be greater than zero and less than or equal to the original payment amount.
    /// </remarks>
    /// <example>150.00</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the ID of the account from which to refund.
    /// </summary>
    /// <remarks>
    /// Must match the account ID of the original payment.
    /// </remarks>
    /// <example>1</example>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the reference number of the original payment transaction.
    /// Used to locate and validate the original payment.
    /// </summary>
    /// <remarks>
    /// The refund transaction will get a reference number in the format: {OriginalRef}-REF
    /// </remarks>
    /// <example>PAY-2024-12-001</example>
    public string ReferenceNo { get; set; } = string.Empty;
}
