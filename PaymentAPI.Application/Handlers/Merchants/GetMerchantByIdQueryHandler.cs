using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Merchants
{
    /// <summary>
    /// Handles the <see cref="GetMerchantByIdQuery"/> to retrieve a specific merchant.
    /// Returns merchant details or NotFound if the merchant doesn't exist.
    /// </summary>
    public class GetMerchantByIdQueryHandler:IRequestHandler<GetMerchantByIdQuery,ApiResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ILogger<GetMerchantByIdQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMerchantByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="merchantRepository">The repository for merchant data access.</param>
        /// <param name="logger">Logger instance.</param>
        public GetMerchantByIdQueryHandler(
            IMerchantRepository merchantRepository,
            ILogger<GetMerchantByIdQueryHandler> logger)
        {
            _merchantRepository = merchantRepository;
            _logger = logger;
        }
        
        /// <summary>
        /// Handles the query by retrieving the merchant with the specified ID.
        /// </summary>
        /// <param name="request">The query containing the merchant ID.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{MerchantDto}"/> containing the merchant data if found,
        /// or a NotFound response if the merchant does not exist.
        /// </returns>
        public async Task<ApiResponse<MerchantDto>> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving merchant - MerchantId: {MerchantId}", request.Id);

            // 1. Fetch Merchant from DB
            var merchant = await _merchantRepository.GetByIdAsync(request.Id, cancellationToken);

            if (merchant == null)
            {
                _logger.LogWarning("Merchant not found - MerchantId: {MerchantId}", request.Id);
                return ApiResponse<MerchantDto>.NotFound("Merchant not found");
            }

            _logger.LogDebug("Merchant retrieved successfully - MerchantId: {MerchantId}, Name: {Name}", 
                merchant.Id, merchant.Name);

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
