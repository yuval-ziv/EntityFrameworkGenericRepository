using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests;

public abstract class BaseTest
{
    private readonly TestsContext _context;
    protected readonly IPersonRepository Repository;

    protected BaseTest(TestsContext context, IPersonRepository repository)
    {
        _context = context;
        Repository = repository;

        Repository.DeleteAll();
    }

    protected void SetUpDatabase(IEnumerable<Person> people)
    {
        _context.People.AddRange(people);
        _context.SaveChanges();
    }

    public static IEnumerable<object[]> PeopleAndAmountDataGenerator()
    {
        yield return new object[]
        {
            NO_PEOPLE,
            NO_PEOPLE.Count
        };
        yield return new object[]
        {
            ONE_PERSON,
            ONE_PERSON.Count
        };
        yield return new object[]
        {
            TWO_PEOPLE,
            TWO_PEOPLE.Count
        };
        yield return new object[]
        {
            TWO_GENERATIONS_OF_PEOPLE,
            TWO_GENERATIONS_OF_PEOPLE.Count
        };
        yield return new object[]
        {
            THREE_GENERATIONS_OF_PEOPLE,
            THREE_GENERATIONS_OF_PEOPLE.Count
        };
    }
}