using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Merchants
{
    public class GetMerchantSummaryQueryHandler : IRequestHandler<GetMerchantSummaryQuery, ApiResponse<MerchantSummaryDto>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        public GetMerchantSummaryQueryHandler(IMerchantRepository merchantRepository,IAccountRepository accountRepository,ITransactionRepository transactionRepository)
        {
            _merchantRepository = merchantRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse<MerchantSummaryDto>> Handle(GetMerchantSummaryQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate merchant
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId, cancellationToken);
            if (merchant == null)
                return ApiResponse<MerchantSummaryDto>.NotFound("Merchant not found");

            // 2. Get accounts
            var allAccounts = await _accountRepository.GetAllAsync(cancellationToken);
            var merchantAccounts = allAccounts.Where(a => a.MerchantId == request.MerchantId).ToList();

            // 3. Get all transactions for each account
            var allTransactions = new List<Transaction>();
            foreach (var acc in merchantAccounts)
            {
                var txns = await _transactionRepository.GetByAccountIdAsync(acc.Id, cancellationToken);
                allTransactions.AddRange(txns);
            }

            // 4. Prepare summary
            var summary = new MerchantSummaryDto
            {
                MerchantId = merchant.Id,
                MerchantName = merchant.Name,
                Email = merchant.Email,
                TotalBalance = merchantAccounts.Sum(a => a.Balance),
                TotalTransactions = allTransactions.Count,
                TotalPayments = allTransactions.Count(t => t.Type == "Payment"),
                TotalRefunds = allTransactions.Count(t => t.Type == "Refund"),
                TotalHolders = merchantAccounts.Count
            };

            return ApiResponse<MerchantSummaryDto>.Success(summary);
        }
    }
}
