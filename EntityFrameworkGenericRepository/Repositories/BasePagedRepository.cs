﻿using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Attributes;
using EntityFrameworkGenericRepository.Collections;
using EntityFrameworkGenericRepository.Entities;
using EntityFrameworkGenericRepository.Utils.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// An abstract implementation of <see cref="IPagedRepository{TEntity,TId,TFilter}"/>.<br/>
/// Basic repositories extending this abstract class should only override <see cref="GetFilterPredicates"/> and <see cref="KeySelector"/>.
/// Repositories of entities with foreign keys must override all Find methods. (<see cref="FindAllPerPage"/>) in order to include related entities.
/// </summary>
/// <remarks>
/// <para>
/// This is a basic implementation of See <see cref="IPagedRepository{TEntity,TId,TFilter}"/>.
/// </para>
/// </remarks>
/// <seealso cref="IPagedRepository{TEntity,TId,TFilter}"/>
/// <seealso cref="BaseRepository{TEntity,TId,TContext}"/>
/// <inheritdoc cref="BaseRepository{TEntity,TId,TContext}"/>
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

    public virtual Expression<Func<TEntity, string>> KeySelector(string orderByColumn)
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));

        MemberExpression orderByColumnProperty = Expression.PropertyOrField(parameterExpression, orderByColumn);

        return (Expression<Func<TEntity, string>>)Expression.Lambda(orderByColumnProperty, parameterExpression);
    }

    public abstract IEnumerable<Expression<Func<TEntity, bool>>> GetFilterPredicates(TFilter filter);

    private IQueryable<TEntity> GetFiltrationQuery(TFilter filter, string orderByColumn, bool orderByAscending, TContext context,
        bool includeRelatedEntities = INCLUDE)
    {
        IQueryable<TEntity> queryable =
            includeRelatedEntities ? context.Set<TEntity>().IncludeMembersWithAttribute(typeof(IncludeAttribute)) : context.Set<TEntity>();

        IQueryable<TEntity> filteredEntities = Filter(queryable, filter);

        Func<Expression<Func<TEntity, string>>, IOrderedQueryable<TEntity>> orderMethod = orderByAscending
            ? filteredEntities.OrderBy
            : filteredEntities.OrderByDescending;

        return orderMethod(KeySelector(orderByColumn));
    }


    private IQueryable<TEntity> Filter(IQueryable<TEntity> queryable, TFilter filter)
    {
        return GetFilterPredicates(filter).Aggregate(queryable, (current, filterPredicate) =>
            current.Where(filterPredicate));
    }

    private static int GetStartIndex(int page, int pageSize) => page * pageSize;
}