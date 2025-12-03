using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;

namespace PaymentAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for RefundRequest entity.
/// Provides data access methods for refund request management and approval workflow.
/// </summary>
public class RefundRequestRepository : GenericRepository<RefundRequest>, IRefundRequestRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefundRequestRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to use for data access.</param>
    public RefundRequestRepository(PaymentDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all pending refund requests with account details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A collection of pending refund requests ordered by request date.</returns>
    public async Task<IEnumerable<RefundRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Account)
            .Where(r => r.Status == "Pending")
            .OrderBy(r => r.RequestDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets refund requests by account ID.
    /// </summary>
    /// <param name="accountId">The account ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A collection of refund requests for the specified account ordered by request date descending.</returns>
    public async Task<IEnumerable<RefundRequest>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.AccountId == accountId)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a pending refund request by original payment reference.
    /// </summary>
    /// <param name="reference">The original payment reference number.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The pending refund request if found; otherwise, null.</returns>
    public async Task<RefundRequest?> GetByOriginalReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.OriginalPaymentReference == reference && r.Status == "Pending", cancellationToken);
    }
}
