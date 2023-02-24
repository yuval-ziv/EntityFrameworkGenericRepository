namespace EntityFrameworkGenericRepository.Utils.ExtensionMethods;

public static class ICollectionExtensionMethods
{
    public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
    {
        return collection == null || collection.Any();
    }
}