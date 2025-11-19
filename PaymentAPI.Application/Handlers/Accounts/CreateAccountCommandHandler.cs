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
    /// <summary>
    /// Handles the <see cref="CreateAccountCommand"/> to create new accounts for merchants.
    /// Validates merchant existence and persists the new account.
    /// </summary>
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ApiResponse<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMerchantRepository _merchantRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAccountCommandHandler"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository for account data access.</param>
        /// <param name="merchantRepository">The repository for merchant data access.</param>
        public CreateAccountCommandHandler(IAccountRepository accountRepository,IMerchantRepository merchantRepository)
        {
            _accountRepository = accountRepository;
            _merchantRepository = merchantRepository;
        }


        /// <summary>
        /// Handles the account creation by validating the merchant and creating the account.
        /// </summary>
        /// <param name="request">The command containing account details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{AccountDto}"/> containing the created account data if successful,
        /// or a NotFound response if the merchant does not exist.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the merchant exists
        /// 2. Creates a new account entity
        /// 3. Persists the account to the database
        /// 4. Returns the account as a DTO
        /// </remarks>
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
