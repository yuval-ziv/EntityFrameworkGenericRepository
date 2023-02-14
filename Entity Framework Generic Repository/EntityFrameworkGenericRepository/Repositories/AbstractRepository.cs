using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Repositories;

public abstract class AbstractRepository<TContext, TId, TEntity> : IRepository<TId, TEntity> where TEntity : BaseEntity 
{
    public IEnumerable<BaseEntity> FindAll()
    {
        throw new NotImplementedException();
    }

    public int Count()
    {
        throw new NotImplementedException();
    }
}