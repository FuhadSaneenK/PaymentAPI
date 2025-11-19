using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Queries.Accounts
{
    /// <summary>
    /// Query to retrieve all accounts associated with a specific merchant.
    /// </summary>
    public class GetAccountsByMerchantIdQuery:IRequest<ApiResponse<List<AccountDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the merchant whose accounts to retrieve.
        /// </summary>
        public int MerchantId { get; set; }

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
