using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    public interface IAccountRepository:IGenericRepository<Account>
    {
    }
}
