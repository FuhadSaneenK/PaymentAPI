using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;

namespace PaymentAPI.Application.Queries.Merchants
{
    /// <summary>
    /// Query to retrieve a specific merchant by their unique identifier.
    /// </summary>
    /// <example>
    /// GET /api/merchant/1
    /// 
    /// Returns:
    /// <code>
    /// {
    ///   "status": 200,
    ///   "message": "Request processed successfully",
    ///   "data": {
    ///     "id": 1,
    ///     "name": "TechMart Electronics",
    ///     "email": "contact@techmart.com"
    ///   }
    /// }
    /// </code>
    /// </example>
    public class GetMerchantByIdQuery : IRequest<ApiResponse<MerchantDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the merchant to retrieve.
        /// </summary>
        /// <example>1</example>
        public int  Id { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMerchantByIdQuery"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the merchant.</param>
        public GetMerchantByIdQuery(int id)
        {
            Id = id;
        }
    }
}
