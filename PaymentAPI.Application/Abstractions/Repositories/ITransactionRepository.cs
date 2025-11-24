using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;

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

        /// <summary>
        /// Retrieves a paginated list of transactions for a specific account with optional filters.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <param name="type">Optional transaction type filter ("Payment" or "Refund").</param>
        /// <param name="status">Optional status filter ("Completed", "Pending", "Failed").</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A paginated result of transactions.</returns>
        Task<PagedResult<Transaction>> GetByAccountIdPagedAsync(
            int accountId,
            DateTime? startDate,
            DateTime? endDate,
            string? type,
            string? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all transactions for all accounts belonging to a specific merchant.
        /// </summary>
        /// <param name="merchantId">The unique identifier of the merchant.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A list of all transactions for the merchant.</returns>
        Task<List<Transaction>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken);
    }
}

