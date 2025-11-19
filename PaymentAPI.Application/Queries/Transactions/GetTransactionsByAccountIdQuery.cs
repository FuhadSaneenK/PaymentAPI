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
    /// <summary>
    /// Query to retrieve all transactions associated with a specific account.
    /// </summary>
    public class GetTransactionsByAccountIdQuery : IRequest<ApiResponse<List<TransactionDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the account whose transactions to retrieve.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsByAccountIdQuery"/> class.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        public GetTransactionsByAccountIdQuery(int accountId)
        {
            AccountId = accountId;
        }   
    }
}
