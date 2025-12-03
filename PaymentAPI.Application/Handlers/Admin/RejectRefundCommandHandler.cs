using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Admin;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Admin;

/// <summary>
/// Handles the rejection of refund requests by admin users.
/// </summary>
/// <remarks>
/// When a refund request is rejected, this handler:
/// 1. Validates the refund request exists and is pending
/// 2. Updates the refund request status to "Rejected"
/// 3. Records the admin user who rejected it, when, and why
/// No transaction is created and the account balance remains unchanged.
/// </remarks>
public class RejectRefundCommandHandler : IRequestHandler<RejectRefundCommand, ApiResponse<object>>
{
    private readonly IRefundRequestRepository _refundRequestRepository;
    private readonly ILogger<RejectRefundCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RejectRefundCommandHandler"/> class.
    /// </summary>
    /// <param name="refundRequestRepository">The repository for refund request data access.</param>
    /// <param name="logger">Logger instance.</param>
    public RejectRefundCommandHandler(
        IRefundRequestRepository refundRequestRepository,
        ILogger<RejectRefundCommandHandler> logger)
    {
        _refundRequestRepository = refundRequestRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the refund rejection command.
    /// </summary>
    /// <param name="request">The command containing rejection details.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="ApiResponse{Object}"/> indicating success or failure of the rejection.
    /// </returns>
    public async Task<ApiResponse<object>> Handle(RejectRefundCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refund rejection - RequestId: {RequestId}, AdminUserId: {AdminUserId}",
            request.RefundRequestId, request.AdminUserId);

        var refundRequest = await _refundRequestRepository.GetByIdAsync(request.RefundRequestId, cancellationToken);
        if (refundRequest == null)
        {
            _logger.LogWarning("Refund rejection failed: Request not found - RequestId: {RequestId}", request.RefundRequestId);
            return ApiResponse<object>.NotFound("Refund request not found");
        }

        if (refundRequest.Status != "Pending")
        {
            _logger.LogWarning("Refund rejection failed: Request already processed - RequestId: {RequestId}, Status: {Status}",
                request.RefundRequestId, refundRequest.Status);
            return ApiResponse<object>.Fail($"Refund request has already been {refundRequest.Status.ToLower()}");
        }

        refundRequest.Status = "Rejected";
        refundRequest.ReviewDate = DateTime.UtcNow;
        refundRequest.ReviewedByUserId = request.AdminUserId;
        refundRequest.AdminComments = request.Reason;

        await _refundRequestRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refund rejected successfully - RequestId: {RequestId}, Reason: {Reason}",
            request.RefundRequestId, request.Reason);

        return ApiResponse<object>.Success(null, "Refund request rejected successfully");
    }
}
