using MediatR;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Admin;

/// <summary>
/// Command to reject a refund request.
/// </summary>
/// <remarks>
/// Only admin users can reject refund requests. The refund request status is updated to "Rejected"
/// and no transaction is created. The account balance remains unchanged.
/// </remarks>
/// <example>
/// <code>
/// {
///   "refundRequestId": 1,
///   "adminUserId": 5,
///   "reason": "Insufficient documentation provided"
/// }
/// </code>
/// </example>
public class RejectRefundCommand : IRequest<ApiResponse<object>>
{
    /// <summary>
    /// Gets or sets the ID of the refund request to reject.
    /// </summary>
    public int RefundRequestId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the admin user rejecting the request.
    /// </summary>
    public int AdminUserId { get; set; }
    
    /// <summary>
    /// Gets or sets the reason for rejection.
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
