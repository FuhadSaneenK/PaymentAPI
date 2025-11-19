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
    /// Repository implementation for managing Transaction entities.
    /// Provides specialized methods for transaction-specific queries in addition to common CRUD operations.
    /// </summary>
    public class TransactionRepository
    : GenericRepository<Transaction>, ITransactionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public TransactionRepository(PaymentDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a transaction by its unique reference number.
        /// </summary>
        /// <param name="referenceNo">The reference number to search for.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The transaction if found; otherwise, null.</returns>
        public async Task<Transaction?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNo, cancellationToken);
        }

        /// <summary>
        /// Retrieves all transactions associated with a specific account.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A list of transactions for the specified account.</returns>
        public async Task<List<Transaction>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync(cancellationToken);
        }
    }
}
