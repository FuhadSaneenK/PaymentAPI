using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Infrastructure.Repositories
{
    public class TransactionRepository
    : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(PaymentDbContext context) : base(context)
        {
        }

        public async Task<Transaction?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNo, cancellationToken);
        }

        public async Task<List<Transaction>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync(cancellationToken);
        }
    }
}
