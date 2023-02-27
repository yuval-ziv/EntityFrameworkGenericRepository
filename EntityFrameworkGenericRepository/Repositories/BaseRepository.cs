using EntityFrameworkGenericRepository.Entities;
using EntityFrameworkGenericRepository.Exceptions;
using EntityFrameworkGenericRepository.Utils.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using static EntityFrameworkGenericRepository.Utils.EntityCollectionUtils;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// An abstract implementation of <see cref="IRepository{TEntity,TId}"/>.<br/>
/// Basic repositories extending this abstract class shouldn't need to override or add any method.<br/>
/// Repositories of entities with foreign keys must override all Find methods. (<see cref="FindById"/>, <see cref = "FindAll"/>, <see cref = "FindAllById"/>) in order to include related entities.
/// </summary>
/// <remarks>
/// <para>
/// This is a basic implementation of See <see cref="IRepository{TEntity,TId}"/>.
/// </para>
/// </remarks>
/// <typeparam name="TContext">A <see cref="DbContext"/> containing the correct <see cref="DbSet{TEntity}"/></typeparam>
/// <typeparam name="TId">id entity type for the repository. Can be a simple <see cref="int"/>, or a complex class.</typeparam>
/// <typeparam name="TEntity">Entity saved in the repository</typeparam>
/// <seealso cref="IRepository{TEntity,TId}"/>
public abstract class BaseRepository<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId> where TId : IEquatable<TId> where TContext : DbContext
{
    private const bool INCLUDE = true;
    protected readonly IDbContextFactory<TContext> ContextFactory;

    protected BaseRepository(IDbContextFactory<TContext> contextFactory)
    {
        ContextFactory = contextFactory;
    }

    public virtual TEntity? FindById(TId id, bool includeRelatedEntities = INCLUDE)
    {
        using TContext context = ContextFactory.CreateDbContext();

        IQueryable<TEntity> queryable =
            includeRelatedEntities ? context.Set<TEntity>().IncludeMembersWithAttribute(typeof(IncludeAttribute)) : context.Set<TEntity>();

        return queryable.SingleOrDefault(entity => entity.Id.Equals(id));
    }

    public virtual ICollection<TEntity> FindAll(bool includeRelatedEntities = INCLUDE)
    {
        using TContext context = ContextFactory.CreateDbContext();

        IQueryable<TEntity> queryable =
            includeRelatedEntities ? context.Set<TEntity>().IncludeMembersWithAttribute(typeof(IncludeAttribute)) : context.Set<TEntity>();

        return queryable.AsNoTracking().ToList();
    }

    public virtual ICollection<TEntity> FindAllById(IEnumerable<TId> ids, bool includeRelatedEntities = INCLUDE)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return Array.Empty<TEntity>();
        }

        using TContext context = ContextFactory.CreateDbContext();

        IQueryable<TEntity> queryable =
            includeRelatedEntities ? context.Set<TEntity>().IncludeMembersWithAttribute(typeof(IncludeAttribute)) : context.Set<TEntity>();

        return queryable.AsNoTracking().Where(entity => idsCollection.Contains(entity.Id)).ToList();
    }

    public virtual long CountAll()
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count();
    }

    public virtual long CountAllById(IEnumerable<TId> ids)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return 0;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count(entity => idsCollection.Contains(entity.Id));
    }

    public virtual bool ExistsById(TId id)
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Any(entity => entity.Id.Equals(id));
    }

    public virtual bool ExistsAllById(IEnumerable<TId> ids)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return true;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count(entity => idsCollection.Contains(entity.Id)) == idsCollection.Count;
    }

    public virtual bool ExistsAnyById(IEnumerable<TId> ids)
    {
        ICollection<TId> idsCollection = ids.ToList();

        if (!idsCollection.Any())
        {
            return true;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Any(entity => idsCollection.Contains(entity.Id));
    }

    public virtual TEntity? Save(TEntity entity)
    {
        TId id = entity.Id;

        if (ExistsById(id))
        {
            throw new EntityWithSameIdExistsException($"Found existing entity with id {id}");
        }

        using TContext context = ContextFactory.CreateDbContext();

        TEntity savedEntity = context.Set<TEntity>().Add(entity).Entity;
        context.SaveChanges();

        return FindById(savedEntity.Id);
    }

    public virtual ICollection<TEntity> SaveAll(IEnumerable<TEntity> entities)
    {
        ICollection<TEntity> entitiesCollection = entities.ToList();

        if (entitiesCollection.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entitiesCollection);

        if (ExistsAnyById(ids))
        {
            throw new EntityWithSameIdExistsException($"At least one of the following entity ids already exists {ids}.");
        }

        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().AddRange(entitiesCollection);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        context.SaveChanges();


        return FindAllById(GetIds<TEntity, TId>(savedEntities));
    }

    public virtual TEntity? Update(TEntity entity)
    {
        TId id = entity.Id;

        if (!ExistsById(id))
        {
            throw new EntityIdDoesNotExistsException($"Could not update entity with id {id} because the id is missing.");
        }

        using TContext context = ContextFactory.CreateDbContext();

        TEntity savedEntity = context.Set<TEntity>().Update(entity).Entity;
        context.SaveChanges();

        return FindById(savedEntity.Id);
    }

    public virtual ICollection<TEntity> UpdateAll(IEnumerable<TEntity> entities)
    {
        ICollection<TEntity> entitiesCollection = entities.ToList();

        if (entitiesCollection.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entitiesCollection);

        if (ExistsAllById(ids))
        {
            throw new EntityIdDoesNotExistsException($"At least one of the following entity ids does not exists {ids}.");
        }

        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().UpdateRange(entitiesCollection);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        context.SaveChanges();

        return FindAllById(GetIds<TEntity, TId>(savedEntities));
    }

    public virtual void Delete(TEntity entity)
    {
        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().Remove(entity);
        context.SaveChanges();
    }

    public virtual void DeleteAll(IEnumerable<TEntity> entities)
    {
        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().RemoveRange(entities);
        context.SaveChanges();
    }

    public virtual void DeleteById(TId id)
    {
        TEntity? entity = FindById(id, false);

        if (entity == null)
        {
            return;
        }

        Delete(entity);
    }

    public virtual void DeleteAllById(IEnumerable<TId> ids)
    {
        ICollection<TEntity> entities = FindAllById(ids, false).ToList();

        DeleteAll(entities);
    }

    public virtual void DeleteAll()
    {
        using TContext context = ContextFactory.CreateDbContext();

        DeleteAll(context.Set<TEntity>());
    }
}