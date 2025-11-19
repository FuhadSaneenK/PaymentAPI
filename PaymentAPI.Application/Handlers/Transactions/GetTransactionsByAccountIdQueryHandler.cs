using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Transactions;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Transactions
{
    /// <summary>
    /// Handles the <see cref="GetTransactionsByAccountIdQuery"/> to retrieve all transactions for a specific account.
    /// Validates account existence and returns the transaction history.
    /// </summary>
    public class GetTransactionsByAccountIdQueryHandler:IRequestHandler<GetTransactionsByAccountIdQuery,ApiResponse<List<TransactionDto>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsByAccountIdQueryHandler"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository for account data access.</param>
        /// <param name="transactionRepository">The repository for transaction data access.</param>
        public GetTransactionsByAccountIdQueryHandler( IAccountRepository accountRepository,ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Handles the query by retrieving all transactions for the specified account.
        /// </summary>
        /// <param name="request">The query containing the account ID.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{List{TransactionDto}}"/> containing the list of transactions if the account exists,
        /// or a NotFound response if the account does not exist.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the account exists
        /// 2. Retrieves all transactions for the account
        /// 3. Maps the transactions to DTOs
        /// </remarks>
        public async Task<ApiResponse<List<TransactionDto>>> Handle(GetTransactionsByAccountIdQuery request,CancellationToken cancellationToken)
        {
            // 1. Validate account
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
                return ApiResponse<List<TransactionDto>>.NotFound("Account not found");

            // 2. Get transactions for this account
            var transactions = await _transactionRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);

            // 3. Map to DTO list
            var dtoList = transactions.Select(t => new TransactionDto
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

            // 4. Return
            return ApiResponse<List<TransactionDto>>.Success(dtoList);
        }
    }
}
