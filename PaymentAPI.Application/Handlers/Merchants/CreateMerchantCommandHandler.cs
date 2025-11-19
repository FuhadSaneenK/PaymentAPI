using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Commands.Merchants;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Merchants
{
    /// <summary>
    /// Handles the <see cref="CreateMerchantCommand"/> to create new merchants.
    /// Validates email uniqueness and persists the merchant entity.
    /// </summary>
    public class CreateMerchantCommandHandler
        : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMerchantCommandHandler"/> class.
        /// </summary>
        /// <param name="merchantRepository">The repository for merchant data access.</param>
        public CreateMerchantCommandHandler(IMerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }
        

        /// <summary>
        /// Handles the merchant creation by validating email uniqueness and creating the merchant.
        /// </summary>
        /// <param name="request">The command containing merchant details.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{MerchantDto}"/> containing the created merchant data if successful,
        /// or a failure response if a merchant with the same email already exists.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the email is unique
        /// 2. Creates a new merchant entity
        /// 3. Persists the merchant to the database
        /// 4. Returns the merchant as a DTO
        /// </remarks>
        public async Task<ApiResponse<MerchantDto>> Handle(CreateMerchantCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if email already exists
            var existingMerchant = await _merchantRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingMerchant != null)
            {
                return ApiResponse<MerchantDto>.Fail("Merchant with this email already exists.");
            }

            // 2. Create entity
            var merchant = new Merchant
            {
                Name = request.Name,
                Email = request.Email
            };

            // 3. Add to DB through repository
            await _merchantRepository.AddAsync(merchant, cancellationToken);

            // 4. Save changes
            await _merchantRepository.SaveChangesAsync(cancellationToken);

            // 5. Map to DTO
            var dto = new MerchantDto
            {
                Id = merchant.Id,
                Name = merchant.Name,
                Email = merchant.Email
            };

            return ApiResponse<MerchantDto>.Created(dto, "Merchant created successfully");
        }
    }
}
