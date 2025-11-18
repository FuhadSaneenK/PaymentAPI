using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    public interface ITransactionRepository :IGenericRepository<Transaction>
    {
        Task<Transaction?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken);
        Task<List<Transaction>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken);
    }
}

