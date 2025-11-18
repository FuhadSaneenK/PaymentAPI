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
    public class PaymentMethodRepository:GenericRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(PaymentDbContext context) : base(context)
        {
        }
    }
}
