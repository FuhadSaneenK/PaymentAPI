using PaymentAPI.Application.Abstractions.Repositories;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Persistance;

namespace PaymentAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing PaymentMethod entities.
/// Inherits all common CRUD operations from <see cref="GenericRepository{T}"/>.
/// </summary>
public class PaymentMethodRepository:GenericRepository<PaymentMethod>, IPaymentMethodRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentMethodRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to use for data access.</param>
    public PaymentMethodRepository(PaymentDbContext context) : base(context)
    {
    }
}
