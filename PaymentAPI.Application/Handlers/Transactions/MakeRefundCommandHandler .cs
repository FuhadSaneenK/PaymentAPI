using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Transactions;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Transactions
{
    /// <summary>
    /// Handles the <see cref="MakeRefundCommand"/> to process refund transactions.
    /// Validates the original payment, ensures refund amount is valid, and updates the account balance.
    /// </summary>
    public class MakeRefundCommandHandler : IRequestHandler<MakeRefundCommand, ApiResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MakeRefundCommandHandler"/> class.
        /// </summary>
        /// <param name="transactionRepository">The repository for transaction data access.</param>
        /// <param name="accountRepository">The repository for account data access.</param>
        public MakeRefundCommandHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }
        
        /// <summary>
        /// Handles the refund command by validating the original payment and creating the refund transaction.
        /// </summary>
        /// <param name="request">The command containing refund details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{TransactionDto}"/> containing the created refund transaction if successful,
        /// or a failure/NotFound response if validation fails.
        /// </returns>
        /// <remarks>
        /// This method performs the following validations and steps:
        /// 1. Validates that the account exists
        /// 2. Validates that the original payment transaction exists
        /// 3. Ensures refund amount does not exceed original payment amount
        /// 4. Checks that no refund already exists for this reference number
        /// 5. Verifies the reference number belongs to the specified account
        /// 6. Creates the refund transaction with "-REF" suffix on reference number
        /// 7. Debits the account balance with the refund amount
        /// 8. Persists all changes to the database
        /// </remarks>
        public async Task<ApiResponse<TransactionDto>> Handle(MakeRefundCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
                return ApiResponse<TransactionDto>.NotFound("Account not found");

            // 2. Find the original payment (same ReferenceNo)
            var originalPayment = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo, cancellationToken);

            if (originalPayment == null || originalPayment.Type != "Payment")
                return ApiResponse<TransactionDto>.Fail("Original payment not found for this reference number");

            // 3. Validate refund amount <= original amount
            if (request.Amount > originalPayment.Amount)
                return ApiResponse<TransactionDto>.Fail("Refund amount cannot exceed original payment amount");

            // 4. Check if a refund already exists for this reference
            var existingRefund = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo + "-REF", cancellationToken);
            if (existingRefund != null)
                return ApiResponse<TransactionDto>.Fail("Refund already processed for this reference number");
            //ensure refund is processed to same account
            if (originalPayment.AccountId != request.AccountId)
                return ApiResponse<TransactionDto>.Fail("Reference number does not belong to this account");

            // 5. Create refund transaction
            var refundTransaction = new Transaction
            {
                Amount = request.Amount,
                AccountId = request.AccountId,
                ReferenceNumber = request.ReferenceNo + "-REF",
                Type = "Refund",
                Status = "Completed",
                Date = DateTime.UtcNow,
                PaymentMethodId = originalPayment.PaymentMethodId
            };
            await _transactionRepository.AddAsync(refundTransaction, cancellationToken);

            // 6. Update account balance
            account.Balance -= request.Amount;
            await _accountRepository.SaveChangesAsync(cancellationToken);

            // 7. Build DTO
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

            return ApiResponse<TransactionDto>.Created(dto, "Refund processed successfully");


        }
    }
}
