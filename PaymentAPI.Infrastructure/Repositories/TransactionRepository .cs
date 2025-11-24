using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;

namespace PaymentAPI.Infrastructure.Repositories;

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

    /// <summary>
    /// Retrieves a paginated list of transactions for a specific account with optional filters.
    /// </summary>
    public async Task<PagedResult<Transaction>> GetByAccountIdPagedAsync(
        int accountId,
        DateTime? startDate,
        DateTime? endDate,
        string? type,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId);

        // Apply filters
        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(t => t.Type == type);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Transaction>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Retrieves all transactions for all accounts belonging to a specific merchant.
    /// Fixes N+1 problem by using a single JOIN query.
    /// </summary>
    public async Task<List<Transaction>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Account)
            .Where(t => t.Account.MerchantId == merchantId)
            .ToListAsync(cancellationToken);
    }
}
