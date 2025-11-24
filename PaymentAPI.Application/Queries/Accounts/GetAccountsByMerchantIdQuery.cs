using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Accounts
{
    /// <summary>
    /// Query to retrieve all accounts associated with a specific merchant.
    /// </summary>
    /// <example>
    /// GET /api/merchant/1/accounts
    /// 
    /// Returns:
    /// <code>
    /// {
    ///   "status": 200,
    ///   "message": "Request processed successfully",
    ///   "data": [
    ///     {
    ///       "id": 1,
    ///       "holderName": "Main Account",
    ///       "balance": 5000.00,
    ///       "merchantId": 1
    ///     },
    ///     {
    ///       "id": 2,
    ///       "holderName": "Savings Account",
    ///       "balance": 10000.00,
    ///       "merchantId": 1
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </example>
    public class GetAccountsByMerchantIdQuery : IRequest<ApiResponse<PagedResult<AccountDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the merchant whose accounts to retrieve.
        /// </summary>
        /// <example>1</example>
        public int MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the page number (1-based).
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size (number of items per page).
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the optional search term to filter by holder name.
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAccountsByMerchantIdQuery"/> class.
        /// </summary>
        /// <param name="merchantId">The unique identifier of the merchant.</param>
        public GetAccountsByMerchantIdQuery(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}
