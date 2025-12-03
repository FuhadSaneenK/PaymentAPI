using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Merchants;

/// <summary>
/// Query to retrieve all merchants in the system.
/// </summary>
/// <remarks>
/// This query should only be accessible to Admin users.
/// Returns a list of all merchants with their basic information.
/// </remarks>
public record GetAllMerchantsQuery : IRequest<ApiResponse<List<MerchantDto>>>;
