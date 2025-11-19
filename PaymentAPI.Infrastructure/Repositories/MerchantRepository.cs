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
    /// Repository implementation for managing Merchant entities.
    /// Provides specialized methods for merchant-specific queries in addition to common CRUD operations.
    /// </summary>
    public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public MerchantRepository(PaymentDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a merchant by their email address using Entity Framework Core query.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The merchant if found; otherwise, null.</returns>
        public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
    }
}
