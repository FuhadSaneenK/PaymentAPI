using MediatR;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Auth;

/// <summary>
/// Command to register a new user in the system.
/// Creates a new user account with hashed password.
/// </summary>
/// <example>
/// <code>
/// {
///   "username": "newuser",
///   "password": "SecurePass123"
/// }
/// </code>
/// </example>
public class RegisterUserCommand : IRequest<ApiResponse<string>>
{
    /// <summary>
    /// Gets or sets the username for the new user account.
    /// </summary>
    /// <remarks>
    /// Must be at least 3 characters long and unique.
    /// </remarks>
    /// <example>newuser</example>
    public string Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password for the new user account (will be hashed before storage).
    /// </summary>
    /// <remarks>
    /// Must be at least 6 characters long.
    /// </remarks>
    /// <example>SecurePass123</example>
    public string Password { get; set; }
}
