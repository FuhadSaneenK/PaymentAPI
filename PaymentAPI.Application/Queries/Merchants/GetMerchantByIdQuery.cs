using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Queries.Merchants
{
    /// <summary>
    /// Query to retrieve a specific merchant by their unique identifier.
    /// </summary>
    public class GetMerchantByIdQuery : IRequest<ApiResponse<MerchantDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the merchant to retrieve.
        /// </summary>
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
