using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Queries.Merchants;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Handlers.Merchants;

/// <summary>
/// Handles the <see cref="GetAllMerchantsQuery"/> to retrieve all merchants.
/// Returns a list of all merchants in the system.
/// </summary>
public class GetAllMerchantsQueryHandler : IRequestHandler<GetAllMerchantsQuery, ApiResponse<List<MerchantDto>>>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly ILogger<GetAllMerchantsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllMerchantsQueryHandler"/> class.
    /// </summary>
    /// <param name="merchantRepository">The repository for merchant data access.</param>
    /// <param name="logger">Logger instance.</param>
    public GetAllMerchantsQueryHandler(
        IMerchantRepository merchantRepository,
        ILogger<GetAllMerchantsQueryHandler> logger)
    {
        _merchantRepository = merchantRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the query by retrieving all merchants from the database.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// An <see cref="ApiResponse{List{MerchantDto}}"/> containing all merchants.
    /// </returns>
    public async Task<ApiResponse<List<MerchantDto>>> Handle(GetAllMerchantsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all merchants");

        try
        {
            // 1. Fetch all merchants from DB
            var merchants = await _merchantRepository.GetAllAsync(cancellationToken);

            // 2. Map to DTOs
            var merchantDtos = merchants.Select(m => new MerchantDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email
            }).ToList();

            _logger.LogDebug("Retrieved {MerchantCount} merchants", merchantDtos.Count);

            // 3. Return Response
            return ApiResponse<List<MerchantDto>>.Success(merchantDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all merchants");
            return ApiResponse<List<MerchantDto>>.Fail("An error occurred while retrieving merchants");
        }
    }
}
