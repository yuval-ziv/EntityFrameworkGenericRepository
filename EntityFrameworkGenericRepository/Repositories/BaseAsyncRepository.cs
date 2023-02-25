using EntityFrameworkGenericRepository.Entities;
using EntityFrameworkGenericRepository.Exceptions;
using EntityFrameworkGenericRepository.Utils.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using static EntityFrameworkGenericRepository.Utils.EntityCollectionUtils;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// An abstract implementation of <see cref="IAsyncRepository{TEntity,TId}"/>.<br/>
/// Basic repositories extending this abstract class shouldn't need to override or add any method.<br/>
/// Repositories of entities with foreign keys must override all Find methods. (<see cref="FindByIdAsync"/>, <see cref = "FindAllAsync"/>, <see cref = "FindAllByIdAsync"/>) in order to include related entities.
/// </summary>
/// <remarks>
/// <para>
/// This is a basic implementation of See <see cref="IAsyncRepository{TEntity,TId}"/>.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">Entity saved in the repository</typeparam>
/// <typeparam name="TId">id entity type for the repository. Can be a simple <see cref="int"/>, or a complex class.</typeparam>
/// <typeparam name="TContext">A <see cref="DbContext"/> containing the correct <see cref="DbSet{TEntity}"/></typeparam>
/// <seealso cref="IAsyncRepository{TEntity,TId}"/>
public abstract class BaseAsyncRepository<TEntity, TId, TContext> : IAsyncRepository<TEntity, TId>
    where TEntity : BaseEntity<TId> where TId : IEquatable<TId> where TContext : DbContext
{
    private const bool INCLUDE = true;
    protected readonly IDbContextFactory<TContext> ContextFactory;

    protected BaseAsyncRepository(IDbContextFactory<TContext> contextFactory)
    {
        ContextFactory = contextFactory;
    }

    public virtual async Task<TEntity?> FindByIdAsync(TId id, bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return context.Set<TEntity>().AsNoTracking().SingleOrDefault(entity => entity.Id.Equals(id));
    }

    public virtual async Task<ICollection<TEntity>> FindAllAsync(bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<ICollection<TEntity>> FindAllByIdAsync(IEnumerable<TId> ids, bool includeRelatedEntities = INCLUDE,
        CancellationToken cancellationToken = default)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return Array.Empty<TEntity>();
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().Where(entity => idsCollection.Contains(entity.Id)).ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<long> CountAllAsync(CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().CountAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<long> CountAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return 0;
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().CountAsync(entity => idsCollection.Contains(entity.Id), cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().AnyAsync(entity => entity.Id.Equals(id), cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> ExistsAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return true;
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().CountAsync(entity => idsCollection.Contains(entity.Id), cancellationToken: cancellationToken) ==
               idsCollection.Count;
    }

    public virtual async Task<bool> ExistsAnyByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return true;
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Set<TEntity>().AsNoTracking().AnyAsync(entity => idsCollection.Contains(entity.Id), cancellationToken: cancellationToken);
    }

    public virtual async Task<TEntity?> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TId id = entity.Id;

        if (await ExistsByIdAsync(id, cancellationToken))
        {
            throw new EntityWithSameIdExistsException($"Found existing entity with id {id}");
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        TEntity savedEntity = context.Set<TEntity>().Add(entity).Entity;
        await context.SaveChangesAsync(cancellationToken);

        return await FindByIdAsync(savedEntity.Id, cancellationToken: cancellationToken);
    }

    public virtual async Task<ICollection<TEntity>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ICollection<TEntity> entitiesCollection = entities.ToList();

        if (entitiesCollection.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entitiesCollection);

        if (await ExistsAnyByIdAsync(ids, cancellationToken))
        {
            throw new EntityWithSameIdExistsException($"At least one of the following entity ids already exists {ids}.");
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        context.Set<TEntity>().AddRange(entitiesCollection);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        await context.SaveChangesAsync(cancellationToken);

        return await FindAllByIdAsync(GetIds<TEntity, TId>(savedEntities), cancellationToken: cancellationToken);
    }

    public virtual async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TId id = entity.Id;

        if (!await ExistsByIdAsync(id, cancellationToken))
        {
            throw new EntityIdDoesNotExistsException($"Could not update entity with id {id} because the id is missing.");
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        TEntity savedEntity = context.Set<TEntity>().Update(entity).Entity;
        await context.SaveChangesAsync(cancellationToken);

        return await FindByIdAsync(savedEntity.Id, cancellationToken: cancellationToken);
    }

    public virtual async Task<ICollection<TEntity>> UpdateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ICollection<TEntity> entitiesCollection = entities.ToList();

        if (entitiesCollection.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entitiesCollection);

        if (await ExistsAllByIdAsync(ids, cancellationToken))
        {
            throw new EntityIdDoesNotExistsException($"At least one of the following entity ids does not exists {ids}.");
        }

        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        context.Set<TEntity>().UpdateRange(entitiesCollection);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        await context.SaveChangesAsync(cancellationToken);

        return await FindAllByIdAsync(GetIds<TEntity, TId>(savedEntities), cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        context.Set<TEntity>().RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await FindByIdAsync(id, false, cancellationToken);

        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, cancellationToken);
    }

    public virtual async Task DeleteAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> entities = await FindAllByIdAsync(ids, false, cancellationToken);

        await DeleteAllAsync(entities, cancellationToken);
    }

    public virtual async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        await DeleteAllAsync(context.Set<TEntity>(), cancellationToken);
    }
}