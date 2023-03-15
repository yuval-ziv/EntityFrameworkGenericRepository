namespace EntityFrameworkGenericRepositoryImplementation.Utils.ExtensionMethods;

public static class IEnumerableExtensionMethods
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> o) where T : class
    {
        return o.Where(x => x != null)!;
    }
}