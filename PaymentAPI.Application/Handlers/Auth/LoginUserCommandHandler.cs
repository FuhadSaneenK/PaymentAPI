using MediatR;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Abstractions.Services;
using PaymentAPI.Application.Commands.Auth;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Handlers.Auth
{
    /// <summary>
    /// Handles the <see cref="LoginUserCommand"/> to authenticate users and generate JWT tokens.
    /// Validates credentials and returns a JWT token upon successful authentication.
    /// </summary>
    public class LoginUserCommandHandler: IRequestHandler<LoginUserCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for user data access.</param>
        /// <param name="passwordHasher">The service for password hash verification.</param>
        /// <param name="jwtService">The service for JWT token generation.</param>
        public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Handles the login command by validating user credentials and generating a JWT token.
        /// </summary>
        /// <param name="request">The login command containing username and password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="ApiResponse{String}"/> containing the JWT token if authentication is successful,
        /// or a failure response with "Invalid credentials" message if authentication fails.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Retrieves the user by username
        /// 2. Verifies the password hash
        /// 3. Generates a JWT token containing user ID, username, and role
        /// </remarks>
        public async Task<ApiResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (user == null)
                return ApiResponse<string>.Fail("Invalid credentials");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                return ApiResponse<string>.Fail("Invalid credentials");

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);

            return ApiResponse<string>.Success(token, "Login successful");
        }
    }

}
