using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for RefundRequest entity operations.
/// Extends common CRUD operations with refund request specific queries.
/// </summary>
public interface IRefundRequestRepository : IGenericRepository<RefundRequest>
{
    /// <summary>
    /// Gets all pending refund requests.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A collection of pending refund requests ordered by request date.</returns>
    Task<IEnumerable<RefundRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets refund requests by account ID.
    /// </summary>
    /// <param name="accountId">The account ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A collection of refund requests for the specified account ordered by request date descending.</returns>
    Task<IEnumerable<RefundRequest>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pending refund request by original payment reference.
    /// </summary>
    /// <param name="reference">The original payment reference number.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The pending refund request if found; otherwise, null.</returns>
    Task<RefundRequest?> GetByOriginalReferenceAsync(string reference, CancellationToken cancellationToken = default);
}
