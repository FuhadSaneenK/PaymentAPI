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
    /// <summary>
    /// Command to process a refund transaction and decrease account balance.
    /// Validates the original payment exists and refund amount does not exceed original payment.
    /// </summary>
    public class MakeRefundCommand:IRequest<ApiResponse<TransactionDto>>
    {
        /// <summary>
        /// Gets or sets the refund amount to be debited from the account.
        /// Must not exceed the original payment amount.
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the account from which to refund.
        /// </summary>
        public int AccountId { get; set; }
        
        /// <summary>
        /// Gets or sets the reference number of the original payment transaction.
        /// Used to locate and validate the original payment.
        /// </summary>
        public string ReferenceNo { get; set; } = string.Empty;
    }
}
