using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Merchants
{
    public class GetMerchantByIdQueryHandler:IRequestHandler<GetMerchantByIdQuery,ApiResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        public GetMerchantByIdQueryHandler(IMerchantRepository merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }
        public async Task<ApiResponse<MerchantDto>> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch Merchant from DB
            var merchant = await _merchantRepository.GetByIdAsync(request.Id, cancellationToken);

            if (merchant == null)
            {
                return ApiResponse<MerchantDto>.NotFound("Merchant not found");
            }

            // 2. Map to DTO
            var dto = new MerchantDto
            {
                Id = merchant.Id,
                Name = merchant.Name,
                Email = merchant.Email
            };

            // 3. Return Response
            return ApiResponse<MerchantDto>.Success(dto);
        }
    }
}
