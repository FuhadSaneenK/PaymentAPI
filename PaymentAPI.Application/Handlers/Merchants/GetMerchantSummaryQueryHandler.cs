using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Merchants
{
    public class GetMerchantSummaryQueryHandler : IRequestHandler<GetMerchantSummaryQuery, ApiResponse<MerchantSummaryDto>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetMerchantSummaryQueryHandler> _logger;

        public GetMerchantSummaryQueryHandler(
            IMerchantRepository merchantRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ILogger<GetMerchantSummaryQueryHandler> logger)
        {
            _merchantRepository = merchantRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<MerchantSummaryDto>> Handle(GetMerchantSummaryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating merchant summary - MerchantId: {MerchantId}", request.MerchantId);

            // 1. Validate merchant
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId, cancellationToken);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found for summary - MerchantId: {MerchantId}", request.MerchantId);
                return ApiResponse<MerchantSummaryDto>.NotFound("Merchant not found");
            }

            try
            {
                // 2. Get accounts
                var merchantAccounts = await _accountRepository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

                // 3. Get all transactions for the merchant in ONE query (fixes N+1 problem)
                var allTransactions = await _transactionRepository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

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

                _logger.LogInformation("Merchant summary generated - MerchantId: {MerchantId}, TotalBalance: {TotalBalance}, TotalAccounts: {TotalAccounts}, TotalTransactions: {TotalTransactions}",
                    merchant.Id, summary.TotalBalance, summary.TotalHolders, summary.TotalTransactions);

                return ApiResponse<MerchantSummaryDto>.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating merchant summary - MerchantId: {MerchantId}", request.MerchantId);
                throw;
            }
        }
    }
}
