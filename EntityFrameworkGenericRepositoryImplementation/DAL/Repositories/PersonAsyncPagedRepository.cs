using System.Linq.Expressions;
using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using EntityFrameworkGenericRepositoryImplementation.Utils.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public class PersonAsyncPagedRepository : BaseAsyncPagedRepository<Person, int, PersonFilter, TestsContext>, IPersonAsyncPagedRepository
{
    public PersonAsyncPagedRepository(IDbContextFactory<TestsContext> contextFactory) : base(contextFactory)
    {
    }

    public override Task<IEnumerable<Expression<Func<Person, bool>>>> GetFilterPredicatesAsync(PersonFilter filter,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<Expression<Func<Person, bool>>?>
        {
            FilterById(filter),
            FilterByName(filter),
            FilterByParentId(filter),
            FilterByParentName(filter),
            FilterByChildrenAmount(filter),
            FilterByChildName(filter)
        }.WhereNotNull());
    }

    private static Expression<Func<Person, bool>>? FilterById(PersonFilter filter)
    {
        if (filter.Id != null)
        {
            return person => person.Id.Equals(filter.Id);
        }

        return null;
    }

    private static Expression<Func<Person, bool>>? FilterByName(PersonFilter filter)
    {
        if (filter.Name != null)
        {
            return person => person.Name.Equals(filter.Name);
        }

        return null;
    }

    private static Expression<Func<Person, bool>>? FilterByParentId(PersonFilter filter)
    {
        if (filter.ParentId != null)
        {
            return person => person.ParentId.Equals(filter.ParentId);
        }

        return null;
    }

    private static Expression<Func<Person, bool>>? FilterByParentName(PersonFilter filter)
    {
        if (filter.ParentName != null)
        {
            return person => person.Parent.Name.Equals(filter.ParentName);
        }

        return null;
    }

    private static Expression<Func<Person, bool>>? FilterByChildrenAmount(PersonFilter filter)
    {
        if (filter.ChildrenAmount != null)
        {
            return person => person.Children.Count.Equals(filter.ChildrenAmount);
        }

        return null;
    }

    private static Expression<Func<Person, bool>>? FilterByChildName(PersonFilter filter)
    {
        if (filter.ChildName != null)
        {
            return person => person.Children.Any(child => child.Name.Equals(filter.ChildName));
        }

        return null;
    }
}