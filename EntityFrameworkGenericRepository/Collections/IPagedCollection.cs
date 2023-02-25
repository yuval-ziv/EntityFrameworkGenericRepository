namespace EntityFrameworkGenericRepository.Collections;

/// <summary>
/// Defines methods to manipulate generic paged collections.
/// </summary>
/// <inheritdoc cref="ICollection{T}"/>
public interface IPagedCollection<T> : ICollection<T>
{
    /// <summary>
    /// The total amount of results the paged collection is taken from.
    /// </summary>
    public int TotalAmount { get; }
}