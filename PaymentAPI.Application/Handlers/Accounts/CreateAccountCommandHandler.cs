using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Accounts;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Accounts
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ApiResponse<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMerchantRepository _merchantRepository;
        public CreateAccountCommandHandler(IAccountRepository accountRepository,IMerchantRepository merchantRepository)
        {
            _accountRepository = accountRepository;
            _merchantRepository = merchantRepository;
        }
        public async Task<ApiResponse<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if Merchant exists
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId, cancellationToken);
            if (merchant == null)
            {
                return ApiResponse<AccountDto>.NotFound("Merchant not found");
            }
            // 2. Create Account
            var account = new Account
            {
                HolderName = request.HolderName,
                Balance = request.Balance,
                MerchantId = request.MerchantId
            };
            // 3. Add to DB
            await _accountRepository.AddAsync(account, cancellationToken);

            // 4. Save
            await _accountRepository.SaveChangesAsync(cancellationToken);
            // 5. Convert to DTO
            var dto = new AccountDto
            {
                Id = account.Id,
                HolderName = account.HolderName,
                Balance = account.Balance,
                MerchantId = account.MerchantId
            };
            // 6. Return response
            return ApiResponse<AccountDto>.Created(dto, "Account created successfully");
        }
    }
}
