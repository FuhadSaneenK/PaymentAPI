using MediatR;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Commands.Transactions
{
   public class MakePaymentCommand:IRequest<ApiResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public int PaymentMethodId { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
    }
}
