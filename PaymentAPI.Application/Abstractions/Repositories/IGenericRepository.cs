using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Abstractions.Repositories
{
    /// <summary>
    /// Generic repository interface providing common CRUD operations for entities.
    /// </summary>
    /// <typeparam name="T">The entity type that this repository manages. Must be a reference type.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
        
        /// <summary>
        /// Retrieves all entities of type T from the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A collection of all entities.</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// Adds a new entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>Changes are not persisted until SaveChangesAsync is called.</remarks>
        Task AddAsync(T entity, CancellationToken cancellationToken);
        
        /// <summary>
        /// Marks an entity for deletion from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>Changes are not persisted until SaveChangesAsync is called.</remarks>
        Task DeleteAsync(T entity, CancellationToken cancellationToken);
        
        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>The number of entities affected by the save operation.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
