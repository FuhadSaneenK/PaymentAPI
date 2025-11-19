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
    /// <summary>
    /// Repository implementation for managing Account entities.
    /// Inherits all common CRUD operations from <see cref="GenericRepository{T}"/>.
    /// </summary>
    public class AccountRepository:GenericRepository<Account>,IAccountRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public AccountRepository(PaymentDbContext context) : base(context)
        {
        }
    }
}
