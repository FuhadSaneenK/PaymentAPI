using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repository interface for managing Merchant entities.
    /// Provides specialized methods for merchant-specific queries in addition to common CRUD operations.
    /// </summary>
    public interface IMerchantRepository : IGenericRepository<Merchant>
    {
        /// <summary>
        /// Retrieves a merchant by their email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The merchant if found; otherwise, null.</returns>
        Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    }
}
