using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Transactions
{
    /// <summary>
    /// Query to retrieve all transactions for a specific account with pagination and filtering.
    /// </summary>
    public class GetTransactionsByAccountIdQuery : IRequest<ApiResponse<PagedResult<TransactionDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the account.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the page number (1-based).
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size (number of items per page).
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the optional start date filter.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the optional end date filter.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the optional transaction type filter ("Payment" or "Refund").
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the optional status filter ("Completed", "Pending", "Failed").
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsByAccountIdQuery"/> class.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        public GetTransactionsByAccountIdQuery(int accountId)
        {
            AccountId = accountId;
        }
    }
}
