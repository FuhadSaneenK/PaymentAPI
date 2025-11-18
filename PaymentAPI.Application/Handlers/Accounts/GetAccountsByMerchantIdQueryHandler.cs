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
    public class GetAccountsByMerchantIdQueryHandler : IRequestHandler<GetAccountsByMerchantIdQuery, ApiResponse<List<AccountDto>>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IAccountRepository _accountRepository;
        public GetAccountsByMerchantIdQueryHandler(IMerchantRepository merchantRepository,IAccountRepository accountRepository)
        {
            _merchantRepository = merchantRepository;
            _accountRepository = accountRepository;
        }
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
