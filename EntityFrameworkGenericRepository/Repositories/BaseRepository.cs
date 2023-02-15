using EntityFrameworkGenericRepository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkGenericRepository.Repositories;

public abstract class BaseRepository<TEntity, TId, TContext> : IRepository<TEntity, TId>
    where TEntity : BaseEntity<TId> where TContext : DbContext where TId : IEquatable<TId>
{
    private readonly IDbContextFactory<TContext> _contextFactory;

    protected BaseRepository(IDbContextFactory<TContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public TEntity? FindById(TId id)
    {
        using TContext context = _contextFactory.CreateDbContext();
        DbSet<TEntity> dbSet = context.Set<TEntity>();

        return dbSet.FirstOrDefault(entity => entity.Id.Equals(id));
    }

    public IEnumerable<TEntity> FindAll()
    {
        using TContext context = _contextFactory.CreateDbContext();
        DbSet<TEntity> dbSet = context.Set<TEntity>();

        return dbSet.AsNoTracking().ToList();
    }

    public int CountAll()
    {
        using TContext context = _contextFactory.CreateDbContext();
        DbSet<TEntity> dbSet = context.Set<TEntity>();

        return dbSet.Count();
    }

    public TEntity Save(TEntity entity)
    {
        using TContext context = _contextFactory.CreateDbContext();
        DbSet<TEntity> dbSet = context.Set<TEntity>();

        TEntity savedEntity = dbSet.Add(entity).Entity;
        context.SaveChanges();

        return savedEntity;
    }

    public void DeleteById(TId id)
    {
        using TContext context = _contextFactory.CreateDbContext();
        DbSet<TEntity> dbSet = context.Set<TEntity>();

        TEntity? entity = FindById(id);

        if (entity == null)
        {
            return;
        }

        dbSet.Remove(entity);
        context.SaveChanges();
    }
}