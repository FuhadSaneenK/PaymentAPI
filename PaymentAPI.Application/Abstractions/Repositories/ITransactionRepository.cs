using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repository interface for managing Transaction entities.
    /// Provides specialized methods for transaction-specific queries in addition to common CRUD operations.
    /// </summary>
    public interface ITransactionRepository :IGenericRepository<Transaction>
    {
        /// <summary>
        /// Retrieves a transaction by its unique reference number.
        /// </summary>
        /// <param name="referenceNo">The reference number to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The transaction if found; otherwise, null.</returns>
        Task<Transaction?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken);
        
        /// <summary>
        /// Retrieves all transactions associated with a specific account.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A list of transactions for the specified account.</returns>
        Task<List<Transaction>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken);
    }
}

