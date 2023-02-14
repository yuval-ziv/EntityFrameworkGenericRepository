using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

public interface IRepository<TEntity, in TId> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    TEntity FindById(TId id);

    IEnumerable<TEntity> FindAll();

    int CountAll();

    TEntity Save(TEntity entity);
}