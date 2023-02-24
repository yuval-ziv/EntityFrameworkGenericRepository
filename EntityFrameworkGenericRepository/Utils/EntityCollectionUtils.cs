using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepository.Utils;

public static class EntityCollectionUtils
{
    public static ICollection<TId> GetIds<TEntity, TId>(IEnumerable<TEntity> entities)
        where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
    {
        return entities.Select(entity => entity.Id).ToList();
    }
}