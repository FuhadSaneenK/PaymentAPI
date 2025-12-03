namespace PaymentAPI.Domain.Entities;

/// <summary>
/// Represents an authenticated user in the system.
/// </summary>
/// <remarks>
/// Users are authenticated using JWT tokens and have role-based access control.
/// Passwords are stored as BCrypt hashes for security.
/// </remarks>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique username for authentication.
    /// </summary>
    /// <remarks>
    /// Must be unique across all users. Minimum 3 characters required.
    /// </remarks>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BCrypt hashed password.
    /// </summary>
    /// <remarks>
    /// Passwords are never stored in plain text. BCrypt hash with salt is used for secure password storage.
    /// </remarks>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role assigned to the user.
    /// </summary>
    /// <remarks>
    /// Valid roles: "Admin" or "User". Default is "User".
    /// Roles are used for authorization and access control.
    /// </remarks>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the foreign key to the merchant this user belongs to.
    /// </summary>
    /// <remarks>
    /// Null for Admin users who can see all merchants.
    /// Required for regular users to restrict them to their merchant's accounts.
    /// </remarks>
    public int? MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the merchant this user belongs to.
    /// </summary>
    /// <remarks>
    /// Navigation property. Null for Admin users.
    /// </remarks>
    public Merchant? Merchant { get; set; }
}
