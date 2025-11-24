using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Accounts;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Accounts
{
    /// <summary>
    /// Handles the <see cref="GetAccountsByMerchantIdQuery"/> to retrieve paginated accounts for a merchant.
    /// Validates merchant existence and returns the list of associated accounts with pagination support.
    /// </summary>
    public class GetAccountsByMerchantIdQueryHandler : IRequestHandler<GetAccountsByMerchantIdQuery, ApiResponse<PagedResult<AccountDto>>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAccountsByMerchantIdQueryHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAccountsByMerchantIdQueryHandler"/> class.
        /// </summary>
        public GetAccountsByMerchantIdQueryHandler(
            IMerchantRepository merchantRepository,
            IAccountRepository accountRepository,
            ILogger<GetAccountsByMerchantIdQueryHandler> logger)
        {
            _merchantRepository = merchantRepository;
            _accountRepository = accountRepository;
            _logger = logger;
        }
        
        /// <summary>
        /// Handles the query by retrieving paginated accounts for the specified merchant.
        /// </summary>
        public async Task<ApiResponse<PagedResult<AccountDto>>> Handle(GetAccountsByMerchantIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving paginated accounts - MerchantId: {MerchantId}, Page: {Page}/{Size}, Search: {Search}",
                request.MerchantId, request.PageNumber, request.PageSize, request.SearchTerm);

            // 1. Check if merchant exists
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId, cancellationToken);
            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found - MerchantId: {MerchantId}", request.MerchantId);
                return ApiResponse<PagedResult<AccountDto>>.NotFound("Merchant not found");
            }

            // 2. Get paginated accounts for merchant
            var pagedAccounts = await _accountRepository.GetByMerchantIdPagedAsync(
                request.MerchantId,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            _logger.LogInformation("Retrieved {Count} accounts (Page {Page} of {TotalPages}, Total: {Total})",
                pagedAccounts.Items.Count, pagedAccounts.PageNumber, 
                pagedAccounts.TotalPages, pagedAccounts.TotalCount);

            // 3. Map to DTO list
            var dtoItems = pagedAccounts.Items.Select(acc => new AccountDto
            {
                Id = acc.Id,
                HolderName = acc.HolderName,
                Balance = acc.Balance,
                MerchantId = acc.MerchantId
            }).ToList();

            var result = new PagedResult<AccountDto>
            {
                Items = dtoItems,
                TotalCount = pagedAccounts.TotalCount,
                PageNumber = pagedAccounts.PageNumber,
                PageSize = pagedAccounts.PageSize
            };

            return ApiResponse<PagedResult<AccountDto>>.Success(result);
        }
    }
}
