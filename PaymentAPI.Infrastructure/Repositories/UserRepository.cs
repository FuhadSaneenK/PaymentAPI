using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing User entities.
    /// Provides specialized methods for user authentication queries in addition to common CRUD operations.
    /// </summary>
    public class UserRepository: GenericRepository<User>, IUserRepository
    {
        private readonly PaymentDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public UserRepository(PaymentDbContext context): base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by their username for authentication purposes.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
