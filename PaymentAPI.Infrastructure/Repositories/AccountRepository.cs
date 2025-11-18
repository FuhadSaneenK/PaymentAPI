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
    public class AccountRepository:GenericRepository<Account>,IAccountRepository
    {
        public AccountRepository(PaymentDbContext context) : base(context)
        {
        }
    }
}
