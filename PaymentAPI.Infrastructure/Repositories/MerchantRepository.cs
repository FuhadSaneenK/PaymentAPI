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
    public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
    {
        public MerchantRepository(PaymentDbContext context) : base(context)
        {
        }

        public async Task<Merchant?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Merchants
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }
    }
}
