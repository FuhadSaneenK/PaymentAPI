using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Merchants
{
    /// <summary>
    /// Query to retrieve a comprehensive summary of merchant operations including aggregated statistics.
    /// </summary>
    /// <remarks>
    /// Returns merchant details along with total balance, account count, transaction count,
    /// payment count, and refund count across all merchant accounts.
    /// </remarks>
    /// <example>
    /// GET /api/merchant/1/summary
    /// 
    /// Returns:
    /// <code>
    /// {
    ///   "status": 200,
    ///   "message": "Request processed successfully",
    ///   "data": {
    ///     "merchantId": 1,
    ///     "merchantName": "TechMart Electronics",
    ///     "email": "contact@techmart.com",
    ///     "totalHolders": 5,
    ///     "totalBalance": 25000.75,
    ///     "totalTransactions": 150,
    ///     "totalPayments": 120,
    ///     "totalRefunds": 30
    ///   }
    /// }
    /// </code>
    /// </example>
    public class GetMerchantSummaryQuery : IRequest<ApiResponse<MerchantSummaryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the merchant for which to generate the summary.
        /// </summary>
        /// <example>1</example>
        public int MerchantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMerchantSummaryQuery"/> class.
        /// </summary>
        /// <param name="merchantId">The unique identifier of the merchant.</param>
        public GetMerchantSummaryQuery(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}
