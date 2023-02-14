namespace EntityFrameworkGenericRepository.Entities;

public abstract class BaseEntity<TId> where TId : IEquatable<TId>
{
    public abstract TId Id { get; set; }
}