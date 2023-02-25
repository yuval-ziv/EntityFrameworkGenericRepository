using System.Collections.ObjectModel;

namespace EntityFrameworkGenericRepository.Collections;

/// <summary>
/// Provides the base class for a generic paged collection.
/// </summary>
/// <inheritdoc cref="Collection{T}"/>
public class PagedCollection<T> : Collection<T>, IPagedCollection<T>
{
    public int TotalAmount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class as a wrapper for the specified list.
    /// </summary>
    public PagedCollection()
    {
        TotalAmount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class as a wrapper for the specified list.
    /// </summary>
    /// <param name="list">the list that is wrapped by the new collection.</param>
    /// <param name="totalAmount">the total amount of results the paged collection is taken from.</param>
    /// <exception cref="ArgumentNullException">list is null.</exception>
    public PagedCollection(IList<T> list, int totalAmount) : base(list)
    {
        TotalAmount = totalAmount;
    }
}