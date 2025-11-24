using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Commands.Auth;

namespace PaymentAPI.Api.Controllers;

/// <summary>
/// Controller for handling user authentication operations.
/// </summary>
/// <remarks>
/// Provides endpoints for user registration and login with JWT token generation.
/// </remarks>
public class AuthController : BaseApiController
{
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AuthController(IMediator mediator, ILogger<AuthController> logger) : base(mediator)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="command">The registration command containing username and password.</param>
    /// <returns>
    /// Returns 200 OK with user details if registration is successful,
    /// or 400 Bad Request if username already exists or validation fails.
    /// </returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Username already exists or validation failed.</response>
    /// <example>
    /// POST /api/auth/register
    /// {
    ///   "username": "newuser",
    ///   "password": "SecurePass123"
    /// }
    /// </example>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        _logger.LogInformation("Registration request received for username: {Username}", command.Username);

        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User registered successfully: {Username}", command.Username);
        }
        else
        {
            _logger.LogWarning("Registration failed for username: {Username}. Reason: {Message}",
                command.Username, response.Message);
        }

        return StatusCode(response.Status, response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="command">The login command containing username and password.</param>
    /// <returns>
    /// Returns 200 OK with JWT token if credentials are valid,
    /// or 400 Bad Request if authentication fails.
    /// </returns>
    /// <response code="200">Login successful, JWT token returned.</response>
    /// <response code="400">Invalid credentials.</response>
    /// <example>
    /// POST /api/auth/login
    /// {
    ///   "username": "existinguser",
    ///   "password": "SecurePass123"
    /// }
    /// </example>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        _logger.LogInformation("Login request received for username: {Username}", command.Username);

        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User logged in successfully: {Username}", command.Username);
        }
        else
        {
            _logger.LogWarning("Login failed for username: {Username}", command.Username);
        }

        return StatusCode(response.Status, response);
    }
}
