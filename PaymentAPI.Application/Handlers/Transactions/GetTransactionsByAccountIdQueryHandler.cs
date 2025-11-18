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
    public class GetTransactionsByAccountIdQueryHandler:IRequestHandler<GetTransactionsByAccountIdQuery,ApiResponse<List<TransactionDto>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        public GetTransactionsByAccountIdQueryHandler( IAccountRepository accountRepository,ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

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
