using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Collections;
using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

public interface IAsyncPagedRepository<TEntity, TId, in TFilter> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    Task<IPagedCollection<TEntity>> FindAllPerPageAsync(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending,
        bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    Task<IEnumerable<Expression<Func<TEntity, bool>>>> GetFilterPredicatesAsync(TFilter filter, CancellationToken cancellationToken = default);

    Task<Expression<Func<TEntity, object>>> KeySelectorAsync(string orderByColumn, CancellationToken cancellationToken = default);
}