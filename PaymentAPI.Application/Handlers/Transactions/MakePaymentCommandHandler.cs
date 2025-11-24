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
    /// Handles the <see cref="MakePaymentCommand"/> to process payment transactions.
    /// Validates account, payment method, and reference number before creating the transaction and updating the account balance.
    /// </summary>
    public class MakePaymentCommandHandler:IRequestHandler<MakePaymentCommand,ApiResponse<TransactionDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<MakePaymentCommandHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MakePaymentCommandHandler"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository for account data access.</param>
        /// <param name="paymentMethodRepository">The repository for payment method data access.</param>
        /// <param name="transactionRepository">The repository for transaction data access.</param>
        /// <param name="logger">Logger instance.</param>
        public MakePaymentCommandHandler(
            IAccountRepository accountRepository,
            IPaymentMethodRepository paymentMethodRepository,
            ITransactionRepository transactionRepository,
            ILogger<MakePaymentCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the payment command by validating prerequisites and creating the payment transaction.
        /// </summary>
        /// <param name="request">The command containing payment details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{TransactionDto}"/> containing the created transaction if successful,
        /// or a failure/NotFound response if validation fails.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the account exists
        /// 2. Validates that the payment method exists
        /// 3. Ensures the reference number is unique
        /// 4. Creates the payment transaction with status "Completed"
        /// 5. Credits the account balance with the payment amount
        /// 6. Persists all changes to the database
        /// </remarks>
        public async Task<ApiResponse<TransactionDto>> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment - AccountId: {AccountId}, Amount: {Amount}, PaymentMethodId: {PaymentMethodId}, Reference: {ReferenceNo}",
                request.AccountId, request.Amount, request.PaymentMethodId, request.ReferenceNo);

            // 1. Validate Account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                _logger.LogWarning("Payment failed: Account not found - AccountId: {AccountId}", request.AccountId);
                return ApiResponse<TransactionDto>.NotFound("Account not found");
            }

            // 2. Validate Payment Method
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(request.PaymentMethodId, cancellationToken);
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment failed: Payment method not found - PaymentMethodId: {PaymentMethodId}", request.PaymentMethodId);
                return ApiResponse<TransactionDto>.NotFound("Payment method not found");
            }

            // 3. Reference number unique check
            var existingTxn = await _transactionRepository.GetByReferenceNoAsync(request.ReferenceNo, cancellationToken);
            if (existingTxn != null)
            {
                _logger.LogWarning("Payment failed: Duplicate reference number - {ReferenceNo}", request.ReferenceNo);
                return ApiResponse<TransactionDto>.Fail("Reference number already exists");
            }

            try
            {
                // 4. Create Transaction
                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    AccountId = request.AccountId,
                    PaymentMethodId = request.PaymentMethodId,
                    ReferenceNumber = request.ReferenceNo,
                    Type = "Payment",
                    Status = "Completed",
                    Date = DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction, cancellationToken);

                // 5. Update account balance
                var oldBalance = account.Balance;
                account.Balance += request.Amount;

                await _accountRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payment completed successfully - TransactionId: {TransactionId}, AccountId: {AccountId}, Amount: {Amount}, PreviousBalance: {OldBalance}, NewBalance: {NewBalance}",
                    transaction.Id, account.Id, request.Amount, oldBalance, account.Balance);

                // 6. Map to DTO
                var dto = new TransactionDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    Status = transaction.Status,
                    ReferenceNo = transaction.ReferenceNumber,
                    AccountId = transaction.AccountId,
                    PaymentMethodId = transaction.PaymentMethodId,
                    Date = transaction.Date
                };

                return ApiResponse<TransactionDto>.Created(dto, "Payment recorded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment - AccountId: {AccountId}, Amount: {Amount}, Reference: {ReferenceNo}",
                    request.AccountId, request.Amount, request.ReferenceNo);
                throw;
            }
        }
    }
}
