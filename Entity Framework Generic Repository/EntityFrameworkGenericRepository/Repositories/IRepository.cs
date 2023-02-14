using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

public interface IRepository<TId, TEntity> where TEntity : BaseEntity
{
    IEnumerable<BaseEntity> FindAll();

    int Count();
    
}