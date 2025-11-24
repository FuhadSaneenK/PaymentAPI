using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Merchants;

/// <summary>
/// Command to create a new merchant in the system.
/// Validates email uniqueness before creating the merchant.
/// </summary>
/// <example>
/// <code>
/// {
///   "name": "TechMart Electronics",
///   "email": "contact@techmart.com",
///   "businessType": "Retail"
/// }
/// </code>
/// </example>
public class CreateMerchantCommand : IRequest<ApiResponse<MerchantDto>>
{
    /// <summary>
    /// Gets or sets the name of the merchant.
    /// </summary>
    /// <example>TechMart Electronics</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email address of the merchant. Must be unique.
    /// </summary>
    /// <example>contact@techmart.com</example>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the business type of the merchant.
    /// </summary>
    /// <example>Retail</example>
    public string BusinessType { get; set; } = string.Empty;
}
