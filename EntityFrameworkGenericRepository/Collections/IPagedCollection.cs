namespace EntityFrameworkGenericRepository.Collections;

public interface IPagedCollection<T> : ICollection<T>
{
    public int TotalAmount { get; }
}