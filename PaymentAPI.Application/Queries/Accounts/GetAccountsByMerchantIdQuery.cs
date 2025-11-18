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
    public class GetAccountsByMerchantIdQuery:IRequest<ApiResponse<List<AccountDto>>>
    {
        public int MerchantId { get; set; }

        public GetAccountsByMerchantIdQuery(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}
