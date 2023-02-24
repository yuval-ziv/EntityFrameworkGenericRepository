using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EntityFrameworkGenericRepository.Entities;
using EntityFrameworkGenericRepository.Exceptions;
using EntityFrameworkGenericRepository.Utils.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using static EntityFrameworkGenericRepository.Utils.EntityCollectionUtils;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// An abstract implementation of <see cref="IRepository(Tid, TEntity)"/>.<br/>
/// Basic repositories extending this abstract class shouldn't need to override or add any method.<br/>
/// Repositories of entities with foreign keys must override all Find methods. (<see cref="FindByld"/>,
///<see cref = "FindAll"/>, <see cref = "FindAllByld"/>).
/// </summary>
/// <typeparam name="TContext">A <see cref="DbContext"/> containing the correct <see cref="DbSet(TEntity}"/></typeparam>
///  <typeparam name="id">id entity type for the repository. Can be a simple <see cref="int"/>, or a
///complex class.</typeparam> /// stypeparam name="TEntity>Entity saved in the repository</typeparam>
/// <seealso cref="Repository(Tid, TEntity)"/>
public abstract class BaseRepository<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId> where TContext : DbContext where TId : IEquatable<TId>
{
    protected readonly IDbContextFactory<TContext> ContextFactory;

    protected BaseRepository(IDbContextFactory<TContext> contextFactory)
    {
        ContextFactory = contextFactory;
    }

    public TEntity? FindById(TId id, bool includeRelatedEntities = true)
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().SingleOrDefault(entity => entity.Id.Equals(id));
    }

    public IEnumerable<TEntity> FindAll(bool includeRelatedEntities = true)
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().ToList();
    }

    public IEnumerable<TEntity> FindAllById(ICollection<TId> ids, bool includeRelatedEntities = true)
    {
        if (!ids.Any())
        {
            return Array.Empty<TEntity>();
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Where(entity => ids.Contains(entity.Id)).ToList();
    }

    public long CountAll()
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count();
    }

    public long CountAllById(ICollection<TId> ids)
    {
        if (!ids.Any())
        {
            return 0;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count(entity => ids.Contains(entity.Id));
    }

    public bool ExistsById(TId id)
    {
        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Any(entity => entity.Id.Equals(id));
    }

    public bool ExistsAllById(ICollection<TId> ids)
    {
        if (!ids.Any())
        {
            return true;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Count(entity => ids.Contains(entity.Id)) == ids.Count;
    }

    public bool ExistsAnyById(ICollection<TId> ids)
    {
        if (!ids.Any())
        {
            return true;
        }

        using TContext context = ContextFactory.CreateDbContext();

        return context.Set<TEntity>().AsNoTracking().Any(entity => ids.Contains(entity.Id));
    }

    public TEntity? Save(TEntity entity)
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

    public IEnumerable<TEntity> SaveAll(ICollection<TEntity> entities)
    {
        if (entities.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entities);

        if (ExistsAnyById(ids))
        {
            throw new EntityWithSameIdExistsException($"At least one of the following entity ids already exists {ids}.");
        }

        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().AddRange(entities);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        context.SaveChanges();


        return FindAllById(GetIds<TEntity, TId>(savedEntities));
    }

    public TEntity? Update(TEntity entity)
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

    public IEnumerable<TEntity> UpdateAll(ICollection<TEntity> entities)
    {
        if (entities.IsNullOrEmpty())
        {
            return Array.Empty<TEntity>();
        }

        ICollection<TId> ids = GetIds<TEntity, TId>(entities);

        if (ExistsAllById(ids))
        {
            throw new EntityIdDoesNotExistsException($"At least one of the following entity ids does not exists {ids}.");
        }

        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().UpdateRange(entities);
        IEnumerable<TEntity> savedEntities = context.Set<TEntity>().Local.AsEnumerable();
        context.SaveChanges();


        return FindAllById(GetIds<TEntity, TId>(savedEntities));
    }

    public void Delete(TEntity entity)
    {
        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().Remove(entity);
        context.SaveChanges();
    }

    public void DeleteAll(IEnumerable<TEntity> entities)
    {
        using TContext context = ContextFactory.CreateDbContext();

        context.Set<TEntity>().RemoveRange(entities);
        context.SaveChanges();
    }

    public void DeleteById(TId id)
    {
        TEntity? entity = FindById(id, false);

        if (entity == null)
        {
            return;
        }

        Delete(entity);
    }

    public void DeleteAllById(ICollection<TId> ids)
    {
        ICollection<TEntity> entities = FindAllById(ids, false).ToList();

        DeleteAll(entities);
    }

    public void DeleteAll()
    {
        using TContext context = ContextFactory.CreateDbContext();

        DeleteAll(context.Set<TEntity>());
    }
}