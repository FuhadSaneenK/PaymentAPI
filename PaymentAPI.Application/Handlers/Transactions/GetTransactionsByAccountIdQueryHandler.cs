using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Transactions;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Transactions
{
    /// <summary>
    /// Handles the <see cref="GetTransactionsByAccountIdQuery"/> to retrieve paginated transactions for a specific account.
    /// Validates account existence and returns the filtered transaction history.
    /// </summary>
    public class GetTransactionsByAccountIdQueryHandler : IRequestHandler<GetTransactionsByAccountIdQuery, ApiResponse<PagedResult<TransactionDto>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetTransactionsByAccountIdQueryHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsByAccountIdQueryHandler"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository for account data access.</param>
        /// <param name="transactionRepository">The repository for transaction data access.</param>
        /// <param name="logger">Logger instance.</param>
        public GetTransactionsByAccountIdQueryHandler(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ILogger<GetTransactionsByAccountIdQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the query by retrieving paginated and filtered transactions for the specified account.
        /// </summary>
        public async Task<ApiResponse<PagedResult<TransactionDto>>> Handle(GetTransactionsByAccountIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving paginated transactions - AccountId: {AccountId}, Page: {Page}/{Size}, Filters: Type={Type}, Status={Status}",
                request.AccountId, request.PageNumber, request.PageSize, request.Type, request.Status);

            // 1. Validate account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                _logger.LogWarning("Account not found - AccountId: {AccountId}", request.AccountId);
                return ApiResponse<PagedResult<TransactionDto>>.NotFound("Account not found");
            }

            // 2. Get paginated transactions with filters
            var pagedTransactions = await _transactionRepository.GetByAccountIdPagedAsync(
                request.AccountId,
                request.StartDate,
                request.EndDate,
                request.Type,
                request.Status,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            _logger.LogInformation("Retrieved {Count} transactions (Page {Page} of {TotalPages}, Total: {Total})",
                pagedTransactions.Items.Count, pagedTransactions.PageNumber, 
                pagedTransactions.TotalPages, pagedTransactions.TotalCount);

            // 3. Map to DTOs
            var dtoItems = pagedTransactions.Items.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Status = t.Status,
                ReferenceNo = t.ReferenceNumber,
                AccountId = t.AccountId,
                PaymentMethodId = t.PaymentMethodId,
                Date = t.Date
            }).ToList();

            var result = new PagedResult<TransactionDto>
            {
                Items = dtoItems,
                TotalCount = pagedTransactions.TotalCount,
                PageNumber = pagedTransactions.PageNumber,
                PageSize = pagedTransactions.PageSize
            };

            return ApiResponse<PagedResult<TransactionDto>>.Success(result);
        }
    }
}
