using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Collections;
using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// Paged repository interface that contains basic methods that should be available to use in all implementations.<br/>
/// Basic implementations can be found in <see cref="BasePagedRepository{TEntity,TId,TFilter,TContext}"/>
/// </summary>
/// <typeparam name="TEntity">Entity saved in the repository.</typeparam>
/// <typeparam name="TFilter">Entity filter.</typeparam>
/// <typeparam name="TId">Id entity type for the repository. Can be a simple <see cref="int"/>,or a complex class. </typeparam>
/// <seealso cref="BasePagedRepository{TEntity,TId,TFilter,TContext}"/>
/// <seealso cref="IAsyncPagedRepository{TEntity,TId,TFilter}"/>
/// <seealso cref="IRepository{TEntity,TId}"/>
public interface IPagedRepository<TEntity, TId, in TFilter> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    /// <summary>
    /// Filters all entities in the table based on <paramref name="filter"/> and returns a subset of the results.
    /// The subset is taken after the filtered entities are sorted by <paramref name="orderByColumn"/>.
    /// The subset ranges from <paramref name="page"/> * <paramref name="pageSize"/> to <paramref name="page"/> * <paramref name="pageSize"/> + <paramref name="pageSize"/>.
    /// </summary>
    /// <param name="filter">an object used for filtering the table.</param>
    /// <param name="page">the number of page from which to start the subset.</param>
    /// <param name="pageSize">the amount of entities in the subset.</param>
    /// <param name="orderByColumn">the name of the column used for ordering the results.</param>
    /// <param name="orderByAscending">if true order the results by ascending value, else by descending value.</param>
    /// <param name="includeRelatedEntities">include related entities if true, else false</param>
    /// <returns>A paged collection containing the subset of results and the <see cref="IPagedCollection{T}.TotalAmount"/> of results matching the given filter.</returns>
    IPagedCollection<TEntity> FindAllPerPage(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending,
        bool includeRelatedEntities = INCLUDE);

    /// <summary>
    /// A list of predicates for the filter parameter of <see cref="FindAllPerPage"/>. Should be a private (or protected) method.
    /// </summary>
    /// <param name="filter">an object used for filtering the table.</param>
    /// <returns>An enumerable of zero or more functions to test each table row for a condition.</returns>
    IEnumerable<Expression<Func<TEntity, bool>>> GetFilterPredicates(TFilter filter);

    //TODO
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderByColumn"></param>
    /// <returns></returns>
    Expression<Func<TEntity, object>> KeySelector(string orderByColumn);
}