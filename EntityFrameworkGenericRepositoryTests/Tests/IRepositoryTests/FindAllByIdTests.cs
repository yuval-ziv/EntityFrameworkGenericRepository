using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class FindAllByIdTests : BaseIRepositoryTest
{
    public FindAllByIdTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(EmptyPeopleDataGenerator))]
    public void When_FindAllByIdCalled_Given_NoPeople_Then_ZeroAmountIsReturned(List<Person> data)
    {
        SetUpDatabase(data);

        ICollection<Person> people = Repository.FindAllById(ALL_PEOPLE_IDS);

        Assert.Equal(0, people.Count);
    }

    [Theory]
    [MemberData(nameof(NonEmptyPeopleDataGenerator))]
    public void When_FindAllByIdCalled_Given_ASingleId_Then_ReturnOnePerson(List<Person> data)
    {
        SetUpDatabase(data);

        ICollection<Person> people = Repository.FindAllById(new[] { A_PERSON_ID });

        Assert.Equal(1, people.Count);
    }

    [Theory]
    [MemberData(nameof(PeopleDataGenerator))]
    public void When_FindAllByIdCalled_Given_AllIds_Then_ReturnAmountOfExistingPeople(List<Person> data)
    {
        SetUpDatabase(data);

        ICollection<Person> people = Repository.FindAllById(ALL_PEOPLE_IDS);

        Assert.Equal(data.Count, people.Count);
    }
}