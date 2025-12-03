using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Commands.Admin;
using PaymentAPI.Application.Queries.Admin;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Controller for admin operations including refund request approval/rejection.
/// </summary>
/// <remarks>
/// All endpoints in this controller require the user to have the "Admin" role.
/// Provides functionality for managing refund requests that require administrative approval.
/// </remarks>
[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly ILogger<AdminController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AdminController(IMediator mediator, ILogger<AdminController> logger) : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all pending refund requests.
    /// </summary>
    /// <returns>
    /// Returns 200 OK with a list of pending refund requests.
    /// </returns>
    /// <response code="200">Successfully retrieved pending refund requests.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <response code="403">Forbidden - Admin role required.</response>
    /// <remarks>
    /// This endpoint is only accessible to users with the "Admin" role.
    /// Returns all refund requests with "Pending" status ordered by request date.
    /// 
    /// Sample response:
    /// <code>
    /// {
    ///   "status": 200,
    ///   "data": [
    ///     {
    ///       "id": 1,
    ///       "amount": 150.00,
    ///       "accountId": 1,
    ///       "originalPaymentReference": "PAY-2024-12-001",
    ///       "reason": "Product defect",
    ///       "status": "Pending",
    ///       "requestDate": "2024-12-10T14:30:00Z"
    ///     }
    ///   ],
    ///   "message": "Retrieved 1 pending refund requests"
    /// }
    /// </code>
    /// </remarks>
    [HttpGet("refund-requests/pending")]
    public async Task<IActionResult> GetPendingRefundRequests()
    {
        _logger.LogInformation("Admin retrieving pending refund requests");
        
        var response = await _mediator.Send(new GetPendingRefundRequestsQuery());
        
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Approves a refund request and processes the refund.
    /// </summary>
    /// <param name="command">The approval command containing refund request ID, admin user ID, and optional comments.</param>
    /// <returns>
    /// Returns 200 OK with the created refund transaction details if approved successfully,
    /// or 400 Bad Request if the request has already been processed,
    /// or 404 Not Found if the refund request doesn't exist.
    /// </returns>
    /// <response code="200">Refund approved and processed successfully.</response>
    /// <response code="400">Refund request has already been processed.</response>
    /// <response code="404">Refund request not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <response code="403">Forbidden - Admin role required.</response>
    /// <remarks>
    /// This endpoint is only accessible to users with the "Admin" role.
    /// When a refund is approved:
    /// - A refund transaction is created with reference "{OriginalRef}-REF"
    /// - The account balance is decreased by the refund amount
    /// - The refund request status is updated to "Approved"
    /// - The admin user ID and timestamp are recorded
    /// 
    /// Sample request:
    /// <code>
    /// POST /api/admin/refund-requests/approve
    /// {
    ///   "refundRequestId": 1,
    ///   "adminUserId": 5,
    ///   "comments": "Approved due to verified product defect"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("refund-requests/approve")]
    public async Task<IActionResult> ApproveRefund([FromBody] ApproveRefundCommand command)
    {
        _logger.LogInformation("Admin approving refund request - RequestId: {RequestId}, AdminUserId: {AdminUserId}",
            command.RefundRequestId, command.AdminUserId);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Refund request approved successfully - RequestId: {RequestId}",
                command.RefundRequestId);
        }
        else
        {
            _logger.LogWarning("Refund approval failed - RequestId: {RequestId}, Reason: {Message}",
                command.RefundRequestId, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Rejects a refund request.
    /// </summary>
    /// <param name="command">The rejection command containing refund request ID, admin user ID, and rejection reason.</param>
    /// <returns>
    /// Returns 200 OK if rejected successfully,
    /// or 400 Bad Request if the request has already been processed,
    /// or 404 Not Found if the refund request doesn't exist.
    /// </returns>
    /// <response code="200">Refund request rejected successfully.</response>
    /// <response code="400">Refund request has already been processed.</response>
    /// <response code="404">Refund request not found.</response>
    /// <response code="401">Unauthorized - JWT token required.</response>
    /// <response code="403">Forbidden - Admin role required.</response>
    /// <remarks>
    /// This endpoint is only accessible to users with the "Admin" role.
    /// When a refund is rejected:
    /// - The refund request status is updated to "Rejected"
    /// - No transaction is created
    /// - The account balance remains unchanged
    /// - The admin user ID, timestamp, and rejection reason are recorded
    /// 
    /// Sample request:
    /// <code>
    /// POST /api/admin/refund-requests/reject
    /// {
    ///   "refundRequestId": 1,
    ///   "adminUserId": 5,
    ///   "reason": "Insufficient documentation provided"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("refund-requests/reject")]
    public async Task<IActionResult> RejectRefund([FromBody] RejectRefundCommand command)
    {
        _logger.LogInformation("Admin rejecting refund request - RequestId: {RequestId}, AdminUserId: {AdminUserId}",
            command.RefundRequestId, command.AdminUserId);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("Refund request rejected successfully - RequestId: {RequestId}",
                command.RefundRequestId);
        }
        else
        {
            _logger.LogWarning("Refund rejection failed - RequestId: {RequestId}, Reason: {Message}",
                command.RefundRequestId, response.Message);
        }
        
        return StatusCode(response.Status, response);
    }
}
