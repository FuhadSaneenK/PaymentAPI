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
    public class GetMerchantByIdQuery : IRequest<ApiResponse<MerchantDto>>
    {
        public int  Id { get; set; }
        public GetMerchantByIdQuery(int id)
        {
            Id = id;
        }
    }
}
