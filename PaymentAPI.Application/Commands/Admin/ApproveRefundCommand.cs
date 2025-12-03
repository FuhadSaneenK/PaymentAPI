using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Admin;

/// <summary>
/// Command to approve a refund request and process the refund.
/// </summary>
/// <remarks>
/// Only admin users can approve refund requests. Upon approval, the refund transaction
/// is created and the account balance is updated immediately.
/// </remarks>
/// <example>
/// <code>
/// {
///   "refundRequestId": 1,
///   "adminUserId": 5,
///   "comments": "Approved due to product defect"
/// }
/// </code>
/// </example>
public class ApproveRefundCommand : IRequest<ApiResponse<TransactionDto>>
{
    /// <summary>
    /// Gets or sets the ID of the refund request to approve.
    /// </summary>
    public int RefundRequestId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the admin user approving the request.
    /// </summary>
    public int AdminUserId { get; set; }
    
    /// <summary>
    /// Gets or sets optional comments from the admin.
    /// </summary>
    public string? Comments { get; set; }
}
