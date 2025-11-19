using PaymentAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repository interface for managing PaymentMethod entities.
    /// Inherits all common CRUD operations from <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IPaymentMethodRepository:IGenericRepository<PaymentMethod>
    {

    }
}
