using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Handlers.Transactions
{
    /// <summary>
    /// Handles the <see cref="MakeRefundCommand"/> to create a refund request that requires admin approval.
    /// </summary>
    /// <remarks>
    /// This handler creates a refund request in "Pending" status rather than immediately processing the refund.
    /// The actual refund transaction is only created when an admin approves the request.
    /// </remarks>
    public class MakeRefundCommandHandler : IRequestHandler<MakeRefundCommand, ApiResponse<RefundRequestDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRefundRequestRepository _refundRequestRepository;
        private readonly ILogger<MakeRefundCommandHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MakeRefundCommandHandler"/> class.
        /// </summary>
        /// <param name="transactionRepository">The repository for transaction data access.</param>
        /// <param name="accountRepository">The repository for account data access.</param>
        /// <param name="refundRequestRepository">The repository for refund request data access.</param>
        /// <param name="logger">Logger instance.</param>
        public MakeRefundCommandHandler(
            ITransactionRepository transactionRepository, 
            IAccountRepository accountRepository,
            IRefundRequestRepository refundRequestRepository,
            ILogger<MakeRefundCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _refundRequestRepository = refundRequestRepository;
            _logger = logger;
        }
        
        /// <summary>
        /// Handles the refund command by creating a refund request for admin approval.
        /// </summary>
        /// <param name="request">The command containing refund request details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{RefundRequestDto}"/> containing the created refund request if successful,
        /// or a failure/NotFound response if validation fails.
        /// </returns>
        /// <remarks>
        /// This method performs the following validations:
        /// 1. Validates that the account exists
        /// 2. Validates that the original payment transaction exists
        /// 3. Ensures refund amount does not exceed original payment amount
        /// 4. Checks that no refund has already been processed for this reference number
        /// 5. Checks that no pending refund request exists for this reference number
        /// 6. Verifies the reference number belongs to the specified account
        /// 7. Creates the refund request in "Pending" status
        /// </remarks>
        public async Task<ApiResponse<RefundRequestDto>> Handle(MakeRefundCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating refund request - AccountId: {AccountId}, Amount: {Amount}, OriginalReference: {ReferenceNo}",
                request.AccountId, request.Amount, request.ReferenceNo);

            // 1. Validate account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                _logger.LogWarning("Refund request failed: Account not found - AccountId: {AccountId}", request.AccountId);
                return ApiResponse<RefundRequestDto>.NotFound("Account not found");
            }

            // 2. Find the original payment
            var originalPayment = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo, cancellationToken);
            if (originalPayment == null || originalPayment.Type != "Payment")
            {
                _logger.LogWarning("Refund request failed: Original payment not found - Reference: {ReferenceNo}", request.ReferenceNo);
                return ApiResponse<RefundRequestDto>.Fail("Original payment not found for this reference number");
            }

            // 3. Validate refund amount
            if (request.Amount > originalPayment.Amount)
            {
                _logger.LogWarning("Refund request failed: Amount exceeds original payment - Reference: {ReferenceNo}, RequestedAmount: {Amount}, OriginalAmount: {OriginalAmount}",
                    request.ReferenceNo, request.Amount, originalPayment.Amount);
                return ApiResponse<RefundRequestDto>.Fail("Refund amount cannot exceed original payment amount");
            }

            // 4. Check if refund already processed
            var existingRefund = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo + "-REF", cancellationToken);
            if (existingRefund != null)
            {
                _logger.LogWarning("Refund request failed: Refund already processed - Reference: {ReferenceNo}", request.ReferenceNo);
                return ApiResponse<RefundRequestDto>.Fail("Refund already processed for this reference number");
            }

            // 5. Check if pending request exists
            var existingRequest = await _refundRequestRepository.GetByOriginalReferenceAsync(request.ReferenceNo, cancellationToken);
            if (existingRequest != null)
            {
                _logger.LogWarning("Refund request failed: Pending request already exists - Reference: {ReferenceNo}", request.ReferenceNo);
                return ApiResponse<RefundRequestDto>.Fail("A pending refund request already exists for this reference number");
            }

            // 6. Ensure refund belongs to same account
            if (originalPayment.AccountId != request.AccountId)
            {
                _logger.LogWarning("Refund request failed: Reference belongs to different account - Reference: {ReferenceNo}, RequestedAccountId: {AccountId}, ActualAccountId: {ActualAccountId}",
                    request.ReferenceNo, request.AccountId, originalPayment.AccountId);
                return ApiResponse<RefundRequestDto>.Fail("Reference number does not belong to this account");
            }

            try
            {
                // 7. Create refund request
                var refundRequest = new RefundRequest
                {
                    Amount = request.Amount,
                    AccountId = request.AccountId,
                    OriginalPaymentReference = request.ReferenceNo,
                    Reason = request.Reason,
                    Status = "Pending",
                    RequestDate = DateTime.UtcNow
                };

                await _refundRequestRepository.AddAsync(refundRequest, cancellationToken);
                await _refundRequestRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Refund request created successfully - RequestId: {RequestId}, AccountId: {AccountId}, Amount: {Amount}",
                    refundRequest.Id, request.AccountId, request.Amount);

                var dto = new RefundRequestDto
                {
                    Id = refundRequest.Id,
                    Amount = refundRequest.Amount,
                    AccountId = refundRequest.AccountId,
                    OriginalPaymentReference = refundRequest.OriginalPaymentReference,
                    Reason = refundRequest.Reason,
                    Status = refundRequest.Status,
                    RequestDate = refundRequest.RequestDate
                };

                return ApiResponse<RefundRequestDto>.Created(dto, "Refund request submitted successfully and is pending admin approval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refund request - AccountId: {AccountId}, Reference: {ReferenceNo}",
                    request.AccountId, request.ReferenceNo);
                throw;
            }
        }
    }
}
