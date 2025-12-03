namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents a merchant in the payment system.
/// </summary>
/// <remarks>
/// A merchant is a business entity that owns multiple accounts and processes transactions.
/// Each merchant must have a unique email address.
/// </remarks>
public class Merchant
{
    /// <summary>
    /// Gets or sets the unique identifier for the merchant.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the merchant's business name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the merchant's unique email address.
    /// </summary>
    /// <remarks>
    /// This email must be unique across all merchants in the system.
    /// </remarks>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the collection of accounts associated with this merchant.
    /// </summary>
    /// <remarks>
    /// Navigation property. A merchant can have multiple accounts.
    /// </remarks>
    public List<Account> Accounts { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of users associated with this merchant.
    /// </summary>
    /// <remarks>
    /// Navigation property. A merchant can have multiple users (employees).
    /// </remarks>
    public List<User> Users { get; set; } = new();
}
