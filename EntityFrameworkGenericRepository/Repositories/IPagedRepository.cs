using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

public interface IPagedRepository<TEntity, TId, in TFilter> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    ICollection<TEntity> FindAllPerPage(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending, out int totalAmount,
        bool includeRelatedEntities = INCLUDE);

    IEnumerable<TEntity> FindAllPerPage(TFilter filter, int page, int pageSize, string orderByColumn, bool orderByAscending,
        bool includeRelatedEntities = INCLUDE);

    IEnumerable<Expression<Func<TEntity, bool>>> GetFilterPredicates(TFilter filter);

    Expression<Func<TEntity, object>> KeySelector(string orderByColumn);
}