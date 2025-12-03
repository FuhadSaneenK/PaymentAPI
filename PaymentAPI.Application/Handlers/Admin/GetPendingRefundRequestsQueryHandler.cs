using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Admin;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Admin;

/// <summary>
/// Handles the query to get all pending refund requests.
/// </summary>
/// <remarks>
/// Returns all refund requests with "Pending" status for admin review.
/// Results are ordered by request date (oldest first).
/// </remarks>
public class GetPendingRefundRequestsQueryHandler : IRequestHandler<GetPendingRefundRequestsQuery, ApiResponse<IEnumerable<RefundRequestDto>>>
{
    private readonly IRefundRequestRepository _refundRequestRepository;
    private readonly ILogger<GetPendingRefundRequestsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPendingRefundRequestsQueryHandler"/> class.
    /// </summary>
    /// <param name="refundRequestRepository">The repository for refund request data access.</param>
    /// <param name="logger">Logger instance.</param>
    public GetPendingRefundRequestsQueryHandler(
        IRefundRequestRepository refundRequestRepository,
        ILogger<GetPendingRefundRequestsQueryHandler> logger)
    {
        _refundRequestRepository = refundRequestRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the query to get pending refund requests.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="ApiResponse{IEnumerable{RefundRequestDto}}"/> containing all pending refund requests.
    /// </returns>
    public async Task<ApiResponse<IEnumerable<RefundRequestDto>>> Handle(GetPendingRefundRequestsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching pending refund requests");

        var pendingRequests = await _refundRequestRepository.GetPendingRequestsAsync(cancellationToken);

        var dtos = pendingRequests.Select(r => new RefundRequestDto
        {
            Id = r.Id,
            Amount = r.Amount,
            AccountId = r.AccountId,
            OriginalPaymentReference = r.OriginalPaymentReference,
            Reason = r.Reason,
            Status = r.Status,
            RequestDate = r.RequestDate,
            ReviewDate = r.ReviewDate,
            ReviewedByUserId = r.ReviewedByUserId,
            AdminComments = r.AdminComments
        }).ToList();

        _logger.LogInformation("Found {Count} pending refund requests", dtos.Count);

        return ApiResponse<IEnumerable<RefundRequestDto>>.Success(dtos, $"Retrieved {dtos.Count} pending refund requests");
    }
}
