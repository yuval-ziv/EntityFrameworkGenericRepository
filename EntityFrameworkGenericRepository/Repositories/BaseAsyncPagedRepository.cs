﻿using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Collections;
using EntityFrameworkGenericRepository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Repositories;

public abstract class BaseAsyncPagedRepository<TEntity, TId, TFilter, TContext> : BaseAsyncRepository<TEntity, TId, TContext>,
    IAsyncPagedRepository<TEntity, TId, TFilter> where TEntity : BaseEntity<TId> where TId : IEquatable<TId> where TContext : DbContext
{
    private const bool INCLUDE = true;

    protected BaseAsyncPagedRepository(IDbContextFactory<TContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<IPagedCollection<TEntity>> FindAllPerPageAsync(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending,
        bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default)
    {
        await using TContext context = await ContextFactory.CreateDbContextAsync(cancellationToken);

        IQueryable<TEntity> orderedEntities = await GetFiltrationQueryAsync(filter, orderByColumn, orderByAscending, context, cancellationToken);
        
        int totalAmount = orderedEntities.Count();

        return new PagedCollection<TEntity>(await orderedEntities.Skip(GetStartIndex(page, pageSize)).Take(pageSize).ToListAsync(cancellationToken: cancellationToken), totalAmount);
    }

    public abstract Task<IEnumerable<Expression<Func<TEntity, bool>>>> GetFilterPredicatesAsync(TFilter filter, CancellationToken cancellationToken = default);

    public abstract Task<Expression<Func<TEntity, object>>> KeySelectorAsync(string orderByColumn, CancellationToken cancellationToken = default);

    private async Task<IQueryable<TEntity>> GetFiltrationQueryAsync(TFilter filter, string orderByColumn, bool orderByAscending, TContext context,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = context.Set<TEntity>();

        IQueryable<TEntity> filteredEntities = await FilterAsync(queryable, filter, cancellationToken);

        Func<Expression<Func<TEntity, object>>, IOrderedQueryable<TEntity>> orderMethod = orderByAscending
            ? filteredEntities.OrderBy
            : filteredEntities.OrderByDescending;

        IQueryable<TEntity> orderedEntities = orderMethod(await KeySelectorAsync(orderByColumn, cancellationToken));

        return orderedEntities;
    }


    private async Task<IQueryable<TEntity>> FilterAsync(IQueryable<TEntity> queryable, TFilter filter, CancellationToken cancellationToken = default)
    {
        return (await GetFilterPredicatesAsync(filter, cancellationToken)).Aggregate(queryable, (current, filterPredicate) =>
            current.Where(filterPredicate));
    }

    private static int GetStartIndex(int page, int pageSize) => page * pageSize;
}