using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Admin;

/// <summary>
/// Query to get all pending refund requests.
/// </summary>
/// <remarks>
/// Returns all refund requests with "Pending" status. Used by admins to review and process refund requests.
/// </remarks>
public class GetPendingRefundRequestsQuery : IRequest<ApiResponse<IEnumerable<RefundRequestDto>>>
{
}
