using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests.Abstraction;

public abstract class BaseTest : IDisposable
{
    protected readonly IDbContextFactory<TestsContext> TestsContextFactory;

    protected BaseTest(IServiceProvider serviceProvider)
    {
        var databaseFilePath = $".//TestDatabases//{Guid.NewGuid()}.sqlite";
        var connectionString = $"Data Source={databaseFilePath}";

        var optionsBuilder = new DbContextOptionsBuilder<TestsContext>();
        optionsBuilder.UseSqlite(connectionString);
        
        TestsContextFactory = new DbContextFactory<TestsContext>(serviceProvider, optionsBuilder.Options, new DbContextFactorySource<TestsContext>());
    }

    protected void SetUpDatabase(IEnumerable<Person> people)
    {
        TestsContext context = TestsContextFactory.CreateDbContext();
        context.People.RemoveRange(context.People);
        context.People.AddRange(people);
        context.SaveChanges();
    }

    public static IEnumerable<object[]> PeopleDataGenerator()
    {
        foreach (object[] obj in EmptyPeopleDataGenerator())
        {
            yield return obj;
        }

        foreach (object[] obj in NonEmptyPeopleDataGenerator())
        {
            yield return obj;
        }
    }

    public static IEnumerable<object[]> EmptyPeopleDataGenerator()
    {
        yield return new object[]
        {
            NO_PEOPLE
        };
    }

    public static IEnumerable<object[]> NonEmptyPeopleDataGenerator()
    {
        yield return new object[]
        {
            ONE_PERSON
        };
        yield return new object[]
        {
            TWO_PEOPLE
        };
        yield return new object[]
        {
            TWO_GENERATIONS_OF_PEOPLE
        };
        yield return new object[]
        {
            THREE_GENERATIONS_OF_PEOPLE
        };
    }

    public void Dispose()
    {
        TestsContext context = TestsContextFactory.CreateDbContext();
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}