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
   /// Command to process a payment transaction and update account balance.
   /// Validates account, payment method, and reference number uniqueness.
   /// </summary>
   public class MakePaymentCommand:IRequest<ApiResponse<TransactionDto>>
    {
        /// <summary>
        /// Gets or sets the payment amount to be credited to the account.
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the account receiving the payment.
        /// </summary>
        public int AccountId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the payment method used for this transaction.
        /// </summary>
        public int PaymentMethodId { get; set; }
        
        /// <summary>
        /// Gets or sets the unique reference number for this payment. Must be unique across all transactions.
        /// </summary>
        public string ReferenceNo { get; set; } = string.Empty;
    }
}
