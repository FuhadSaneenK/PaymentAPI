using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repository interface for managing User entities.
    /// Provides specialized methods for user authentication queries in addition to common CRUD operations.
    /// </summary>
    public interface IUserRepository:IGenericRepository<User>
    {
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    }
}
