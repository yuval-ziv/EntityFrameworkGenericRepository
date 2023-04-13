using System.Collections.ObjectModel;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryTests.DataGenerators;

public class PeopleData
{
    public const int A_PERSON_ID = 1;
    public const int ANOTHER_PERSON_ID = 2;
    public const int A_THIRD_PERSON_ID = 3;

    public readonly static ReadOnlyCollection<int> ALL_PEOPLE_IDS = new(new List<int> { A_PERSON_ID, ANOTHER_PERSON_ID, A_THIRD_PERSON_ID });

    public readonly static List<Person> NO_PEOPLE = new();

    public readonly static List<Person> ONE_PERSON = new()
    {
        new Person(A_PERSON_ID, "Person One")
    };

    public readonly static List<Person> TWO_PEOPLE = new()
    {
        new Person(A_PERSON_ID, "Person One"),
        new Person(ANOTHER_PERSON_ID, "Person Two")
    };

    public readonly static List<Person> TWO_GENERATIONS_OF_PEOPLE = new()
    {
        new Person(A_PERSON_ID, "First Generation"),
        new Person(ANOTHER_PERSON_ID, "Second Generation", A_PERSON_ID)
    };

    public readonly static List<Person> THREE_GENERATIONS_OF_PEOPLE = new()
    {
        new Person(A_PERSON_ID, "First Generation"),
        new Person(ANOTHER_PERSON_ID, "Second Generation", A_PERSON_ID),
        new Person(A_THIRD_PERSON_ID, "Third Generation", ANOTHER_PERSON_ID)
    };
}