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
    public class CreateMerchantCommandHandler
        : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        public CreateMerchantCommandHandler(IMerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }

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
