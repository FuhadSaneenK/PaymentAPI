using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repository interface for managing Account entities.
    /// Inherits all common CRUD operations from <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IAccountRepository:IGenericRepository<Account>
    {
        /// <summary>
        /// Retrieves all accounts associated with a specific merchant.
        /// </summary>
        /// <param name="merchantId">The unique identifier of the merchant.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A list of accounts for the specified merchant.</returns>
        Task<List<Account>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a paginated list of accounts for a specific merchant with optional search.
        /// </summary>
        /// <param name="merchantId">The unique identifier of the merchant.</param>
        /// <param name="searchTerm">Optional search term to filter by holder name.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A paginated result of accounts.</returns>
        Task<PagedResult<Account>> GetByMerchantIdPagedAsync(
            int merchantId,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
