using System.Collections.ObjectModel;

namespace EntityFrameworkGenericRepository.Collections;

public class PagedCollection<T> : Collection<T>, IPagedCollection<T>
{
    public int TotalAmount { get; }

    public PagedCollection()
    {
        TotalAmount = 0;
    }

    public PagedCollection(IList<T> list, int totalAmount) : base(list)
    {
        TotalAmount = totalAmount;
    }
}