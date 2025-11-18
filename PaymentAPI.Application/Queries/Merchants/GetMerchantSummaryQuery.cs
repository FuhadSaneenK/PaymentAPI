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
    public class GetMerchantSummaryQuery : IRequest<ApiResponse<MerchantSummaryDto>>
    {
        public int MerchantId { get; set; }

        public GetMerchantSummaryQuery(int merchantId)
        {
            MerchantId = merchantId;
        }
    }
}
