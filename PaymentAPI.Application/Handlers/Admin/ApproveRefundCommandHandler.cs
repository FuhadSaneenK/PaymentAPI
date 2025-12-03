using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Admin;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Handlers.Admin;

/// <summary>
/// Handles the approval of refund requests by admin users.
/// </summary>
/// <remarks>
/// When a refund request is approved, this handler:
/// 1. Validates the refund request exists and is pending
/// 2. Creates the actual refund transaction
/// 3. Updates the account balance
/// 4. Updates the refund request status to "Approved"
/// 5. Records the admin user who approved it and when
/// </remarks>
public class ApproveRefundCommandHandler : IRequestHandler<ApproveRefundCommand, ApiResponse<TransactionDto>>
{
    private readonly IRefundRequestRepository _refundRequestRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<ApproveRefundCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApproveRefundCommandHandler"/> class.
    /// </summary>
    /// <param name="refundRequestRepository">The repository for refund request data access.</param>
    /// <param name="transactionRepository">The repository for transaction data access.</param>
    /// <param name="accountRepository">The repository for account data access.</param>
    /// <param name="logger">Logger instance.</param>
    public ApproveRefundCommandHandler(
        IRefundRequestRepository refundRequestRepository,
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ILogger<ApproveRefundCommandHandler> logger)
    {
        _refundRequestRepository = refundRequestRepository;
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the refund approval command.
    /// </summary>
    /// <param name="request">The command containing approval details.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="ApiResponse{TransactionDto}"/> containing the created refund transaction if successful,
    /// or a failure/NotFound response if validation fails.
    /// </returns>
    public async Task<ApiResponse<TransactionDto>> Handle(ApproveRefundCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refund approval - RequestId: {RequestId}, AdminUserId: {AdminUserId}",
            request.RefundRequestId, request.AdminUserId);

        // 1. Get refund request
        var refundRequest = await _refundRequestRepository.GetByIdAsync(request.RefundRequestId, cancellationToken);
        if (refundRequest == null)
        {
            _logger.LogWarning("Refund approval failed: Request not found - RequestId: {RequestId}", request.RefundRequestId);
            return ApiResponse<TransactionDto>.NotFound("Refund request not found");
        }

        // 2. Validate status
        if (refundRequest.Status != "Pending")
        {
            _logger.LogWarning("Refund approval failed: Request already processed - RequestId: {RequestId}, Status: {Status}",
                request.RefundRequestId, refundRequest.Status);
            return ApiResponse<TransactionDto>.Fail($"Refund request has already been {refundRequest.Status.ToLower()}");
        }

        // 3. Get account
        var account = await _accountRepository.GetByIdAsync(refundRequest.AccountId, cancellationToken);
        if (account == null)
        {
            _logger.LogWarning("Refund approval failed: Account not found - AccountId: {AccountId}", refundRequest.AccountId);
            return ApiResponse<TransactionDto>.NotFound("Account not found");
        }

        // 4. Get original payment
        var originalPayment = await _transactionRepository.GetByReferenceNoAsync(refundRequest.OriginalPaymentReference, cancellationToken);
        if (originalPayment == null)
        {
            _logger.LogWarning("Refund approval failed: Original payment not found - Reference: {Reference}",
                refundRequest.OriginalPaymentReference);
            return ApiResponse<TransactionDto>.NotFound("Original payment not found");
        }

        try
        {
            // 5. Create refund transaction
            var refundTransaction = new Transaction
            {
                Amount = refundRequest.Amount,
                AccountId = refundRequest.AccountId,
                ReferenceNumber = refundRequest.OriginalPaymentReference + "-REF",
                Type = "Refund",
                Status = "Completed",
                Date = DateTime.UtcNow,
                PaymentMethodId = originalPayment.PaymentMethodId
            };

            await _transactionRepository.AddAsync(refundTransaction, cancellationToken);

            // 6. Update account balance
            var oldBalance = account.Balance;
            account.Balance -= refundRequest.Amount;

            // IMPORTANT: Save changes first to generate the transaction ID from the database
            await _accountRepository.SaveChangesAsync(cancellationToken);

            // 7. Now update refund request with the generated transaction ID
            refundRequest.Status = "Approved";
            refundRequest.ReviewDate = DateTime.UtcNow;
            refundRequest.ReviewedByUserId = request.AdminUserId;
            refundRequest.AdminComments = request.Comments;
            refundRequest.RefundTransactionId = refundTransaction.Id; // Now has a valid ID

            // 8. Save the refund request updates
            await _refundRequestRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refund approved and processed - RequestId: {RequestId}, TransactionId: {TransactionId}, Amount: {Amount}, PreviousBalance: {OldBalance}, NewBalance: {NewBalance}",
                request.RefundRequestId, refundTransaction.Id, refundRequest.Amount, oldBalance, account.Balance);

            var dto = new TransactionDto
            {
                Id = refundTransaction.Id,
                Amount = refundTransaction.Amount,
                Type = refundTransaction.Type,
                Status = refundTransaction.Status,
                ReferenceNo = refundTransaction.ReferenceNumber,
                AccountId = refundTransaction.AccountId,
                PaymentMethodId = refundTransaction.PaymentMethodId,
                Date = refundTransaction.Date
            };

            return ApiResponse<TransactionDto>.Success(dto, "Refund approved and processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving refund - RequestId: {RequestId}", request.RefundRequestId);
            throw;
        }
    }
}
