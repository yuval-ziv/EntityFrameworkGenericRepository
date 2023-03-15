using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryTests.DataGenerators;

public class PeopleData
{
    public static readonly List<Person> NO_PEOPLE = new();

    public static readonly List<Person> ONE_PERSON = new()
    {
        new Person(1, "Person One")
    };

    public static readonly List<Person> TWO_PEOPLE = new()
    {
        new Person(1, "Person One"),
        new Person(2, "Person Two")
    };

    public static readonly List<Person> TWO_GENERATIONS_OF_PEOPLE = new()
    {
        new Person(1, "First Generation"),
        new Person(2, "Second Generation", 1),
    };

    public static readonly List<Person> THREE_GENERATIONS_OF_PEOPLE = new()
    {
        new Person(1, "First Generation"),
        new Person(2, "Second Generation", 1),
        new Person(3, "Third Generation", 2),
    };

    public static void SetUpDatabase(TestsContext context, IEnumerable<Person> people)
    {
        context.People.AddRange(people);
        context.SaveChanges();
    }
}