using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Queries.Transactions
{
    public class GetTransactionsByAccountIdQuery : IRequest<ApiResponse<List<TransactionDto>>>
    {
        public int AccountId { get; set; }

        public GetTransactionsByAccountIdQuery(int accountId)
        {
            AccountId = accountId;
        }   
    }
}
