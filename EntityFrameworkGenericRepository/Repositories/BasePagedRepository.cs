using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// An abstract implementation of <see cref="IPagedRepository{TEntity,TId,TFilter}"/>.<br/>
/// Basic repositories extending this abstract class shouldn only override <see cref="GetFilterPredicates"/> and <see cref="KeySelector"/>.<br/>
///</summary>
/// <typeparam name="TContext">A <see cref="DbContext"/> containing the correct <see cref="DbSet{TEntity}"/></typeparam>
/// <typeparam name="TId">ld entity type for the repository. Can be a simple <see cref="int"/>,or a complex class. </typeparam>
/// <typeparam name="TEntity">Entity saved in the repository.</typeparam> /// <typeparam name="TFilter">Entity filter.</typeparam>
///<seealso cref="IPagedRepository{TEntity,TId,TFilter}"/>
public abstract class BasePagedRepository<TEntity, TId, TFilter, TContext> : BaseRepository<TEntity, TId, TContext>, IPagedRepository<TEntity, TId, TFilter>
    where TEntity : BaseEntity<TId> where TId : IEquatable<TId> where TContext : DbContext
{
    private const bool INCLUDE = true;

    protected BasePagedRepository(IDbContextFactory<TContext> contextFactory) : base(contextFactory)
    {
    }

    public virtual IPagedCollection<TEntity> FindAllPerPage(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending,
        bool includeRelatedEntities = INCLUDE)
    {
        using TContext context = ContextFactory.CreateDbContext();

        IQueryable<TEntity> orderedEntities = GetFiltrationQuery(filter, orderByColumn, orderByAscending, context, includeRelatedEntities);

        int totalAmount = orderedEntities.Count();

        return new PagedCollection<TEntity>(orderedEntities.Skip(GetStartIndex(page, pageSize)).Take(pageSize).ToList(), totalAmount);
    }

    public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetFilterPredicates(TFilter filter);

    public abstract Expression<Func<TEntity, object>> KeySelector(string orderByColumn);

    private IQueryable<TEntity> GetFiltrationQuery(TFilter filter, string orderByColumn, bool orderByAscending, TContext context,
        bool includeRelatedEntities = INCLUDE)
    {
        IQueryable<TEntity> queryable = context.Set<TEntity>();

        IQueryable<TEntity> filteredEntities = Filter(queryable, filter);

        Func<Expression<Func<TEntity, object>>, IOrderedQueryable<TEntity>> orderMethod = orderByAscending
            ? filteredEntities.OrderBy
            : filteredEntities.OrderByDescending;

        IQueryable<TEntity> orderedEntities = orderMethod(KeySelector(orderByColumn));

        return orderedEntities;
    }


    private IQueryable<TEntity> Filter(IQueryable<TEntity> queryable, TFilter filter)
    {
        return GetFilterPredicates(filter).Aggregate(queryable, (current, filterPredicate) =>
            current.Where(filterPredicate));
    }

    private static int GetStartIndex(int page, int pageSize) => page * pageSize;
}