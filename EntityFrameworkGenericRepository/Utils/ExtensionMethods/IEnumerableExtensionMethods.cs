namespace EntityFrameworkGenericRepository.Utils.ExtensionMethods;

public static class IEnumerableExtensionMethods
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || collection.Any();
    }
}