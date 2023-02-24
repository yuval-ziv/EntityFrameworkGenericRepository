using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// Async version of <see cref="IRepository{TEntity,TId}"/>
/// </summary>
/// <typeparam name="TId">ld entity type for the repository. Can be a simple <see cref="int"/>, or a complex class.</typeparam>
/// <typeparam name="TEntity">Entity saved in the repository.</typeparam>
/// <seealso cref="BaseAsyncRepository{TContext, Tid, TEntity}"/>
/// <seealso cref="IRepository{TEntity,TId}"/>
/// <seealso cref="IAsyncPagedRepository{TEntity,TId,TFilter}"/>
public interface IAsyncRepository<TEntity, in TId> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    Task<TEntity?> FindByIdAsync(TId id, bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> FindAllAsync(bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> FindAllByIdAsync(IEnumerable<TId> ids, bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    Task<long> CountAllAsync(CancellationToken cancellationToken = default);

    Task<long> CountAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task<bool> ExistsAnyByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task<TEntity?> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> UpdateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task DeleteAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}