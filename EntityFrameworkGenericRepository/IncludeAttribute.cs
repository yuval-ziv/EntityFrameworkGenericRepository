namespace EntityFrameworkGenericRepository;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class IncludeAttribute : Attribute
{
}