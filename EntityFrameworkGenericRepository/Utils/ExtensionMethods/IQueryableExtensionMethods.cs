using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Utils.ExtensionMethods;

public static class IQueryableExtensionMethods
{
    public static IQueryable<T> IncludeMembersWithAttribute<T>(this DbSet<T> queryable, Type attributeType) where T : class
    {
        Type entityType = typeof(T);
        List<MemberInfo> members = entityType.GetProperties().Cast<MemberInfo>().ToList();
        members.AddRange(entityType.GetFields().Cast<MemberInfo>().ToList());

        IEnumerable<MemberInfo> includedMembers = members.Where(member => Attribute.IsDefined(member, attributeType));

        return includedMembers.Aggregate(queryable.AsQueryable(), (current, member) => current.Include(member.Name));
    }
}