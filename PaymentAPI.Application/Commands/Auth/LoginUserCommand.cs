using MediatR;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Commands.Auth;

/// <summary>
/// Command to authenticate a user and generate a JWT token.
/// </summary>
/// <example>
/// <code>
/// {
///   "username": "admin",
///   "password": "Admin@123"
/// }
/// </code>
/// </example>
public class LoginUserCommand : IRequest<ApiResponse<string>>
{
    /// <summary>
    /// Gets or sets the username of the user attempting to log in.
    /// </summary>
    /// <example>admin</example>
    public string Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password of the user attempting to log in.
    /// </summary>
    /// <example>Admin@123</example>
    public string Password { get; set; }
}
