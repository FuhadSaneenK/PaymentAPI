using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Application.Wrappers;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;

namespace PaymentAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Account entities.
/// Inherits all common CRUD operations from <see cref="GenericRepository{T}"/>.
/// </summary>
public class AccountRepository:GenericRepository<Account>,IAccountRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to use for data access.</param>
    public AccountRepository(PaymentDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves all accounts associated with a specific merchant.
    /// </summary>
    public async Task<List<Account>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.MerchantId == merchantId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of accounts for a specific merchant with optional search.
    /// </summary>
    public async Task<PagedResult<Account>> GetByMerchantIdPagedAsync(
        int merchantId,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Accounts
            .AsNoTracking()
            .Where(a => a.MerchantId == merchantId);

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.HolderName.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.HolderName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Account>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
