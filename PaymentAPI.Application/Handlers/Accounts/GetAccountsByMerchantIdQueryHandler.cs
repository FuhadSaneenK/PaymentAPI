using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Accounts;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Accounts
{
    /// <summary>
    /// Handles the <see cref="GetAccountsByMerchantIdQuery"/> to retrieve all accounts for a merchant.
    /// Validates merchant existence and returns the list of associated accounts.
    /// </summary>
    public class GetAccountsByMerchantIdQueryHandler : IRequestHandler<GetAccountsByMerchantIdQuery, ApiResponse<List<AccountDto>>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAccountRepository _accountRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAccountsByMerchantIdQueryHandler"/> class.
        /// </summary>
        /// <param name="merchantRepository">The repository for merchant data access.</param>
        /// <param name="accountRepository">The repository for account data access.</param>
        public GetAccountsByMerchantIdQueryHandler(IMerchantRepository merchantRepository,IAccountRepository accountRepository)
        {
            _merchantRepository = merchantRepository;
            _accountRepository = accountRepository;
        }
        
        /// <summary>
        /// Handles the query by retrieving and filtering accounts for the specified merchant.
        /// </summary>
        /// <param name="request">The query containing the merchant ID.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{List{AccountDto}}"/> containing the list of accounts if the merchant exists,
        /// or a NotFound response if the merchant does not exist.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the merchant exists
        /// 2. Retrieves all accounts and filters by merchant ID
        /// 3. Maps the accounts to DTOs
        /// </remarks>
        public async Task<ApiResponse<List<AccountDto>>> Handle( GetAccountsByMerchantIdQuery request,CancellationToken cancellationToken)
        {
            // 1. Check if merchant exists
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId, cancellationToken);
            if (merchant == null)
            {
                return ApiResponse<List<AccountDto>>.NotFound("Merchant not found");
            }

            // 2. Get accounts for merchant
            var accounts = (await _accountRepository.GetAllAsync(cancellationToken))
                .Where(a => a.MerchantId == request.MerchantId)
                .ToList();

            // 3. Map to DTO list
            var dtoList = accounts.Select(acc => new AccountDto
            {
                Id = acc.Id,
                HolderName = acc.HolderName,
                Balance = acc.Balance,
                MerchantId = acc.MerchantId
            }).ToList();

            // 4. Return success
            return ApiResponse<List<AccountDto>>.Success(dtoList);

        }
    }
}
