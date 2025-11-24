using MediatR;
using Microsoft.Extensions.Logging;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Abstractions.Services;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Handlers.Auth
{
    /// <summary>
    /// Handles the <see cref="RegisterUserCommand"/> to create new user accounts.
    /// Validates username uniqueness and securely stores user credentials.
    /// </summary>
    public class RegisterUserCommandHandler: IRequestHandler<RegisterUserCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for user data access.</param>
        /// <param name="passwordHasher">The service for password hashing.</param>
        /// <param name="logger">Logger instance.</param>
        public RegisterUserCommandHandler(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Handles the user registration by creating a new user account with hashed password.
        /// </summary>
        /// <param name="request">The registration command containing username and password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{String}"/> with success message if registration is successful,
        /// or a failure response if username already exists.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that the username is unique
        /// 2. Hashes the password using BCrypt
        /// 3. Creates and persists the new user entity
        /// </remarks>
        public async Task<ApiResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration attempt for username: {Username}", request.Username);

            var existing = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existing != null)
            {
                _logger.LogWarning("Registration failed: Username already exists - {Username}", request.Username);
                return ApiResponse<string>.Fail("Username already exists");
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            try
            {
                await _userRepository.AddAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User registered successfully - UserId: {UserId}, Username: {Username}", 
                    user.Id, user.Username);

                return ApiResponse<string>.Success("User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Username}", request.Username);
                throw;
            }
        }
    }

}
