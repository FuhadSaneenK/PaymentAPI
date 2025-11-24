namespace PaymentAPI.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a merchant.
/// </summary>
/// <remarks>
/// Used for transferring merchant data between layers without exposing domain entities.
/// </remarks>
/// <example>
/// <code>
/// {
///   "id": 1,
///   "name": "TechMart Electronics",
///   "email": "contact@techmart.com"
/// }
/// </code>
/// </example>
public class MerchantDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the merchant.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the business name of the merchant.
    /// </summary>
    /// <example>TechMart Electronics</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique email address of the merchant.
    /// </summary>
    /// <remarks>
    /// Email must be unique across all merchants.
    /// </remarks>
    /// <example>contact@techmart.com</example>
    public string Email { get; set; } = string.Empty;
}
