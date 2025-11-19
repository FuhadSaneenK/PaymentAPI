using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Abstractions.Services;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for user data access.</param>
        /// <param name="passwordHasher">The service for password hashing.</param>
        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
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
            var existing = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existing != null)
                return ApiResponse<string>.Fail("Username already exists");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = _passwordHasher.Hash(request.Password)
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.Success("User registered successfully");
        }
    }

}
