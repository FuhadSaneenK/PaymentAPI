using MediatR;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Commands.Auth
{
    /// <summary>
    /// Command to register a new user in the system.
    /// Creates a new user account with hashed password.
    /// </summary>
    public class RegisterUserCommand : IRequest<ApiResponse<string>>
    {
        /// <summary>
        /// Gets or sets the username for the new user account.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Gets or sets the password for the new user account (will be hashed before storage).
        /// </summary>
        public string Password { get; set; }
    }
}
