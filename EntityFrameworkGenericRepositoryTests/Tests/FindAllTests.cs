using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

namespace EntityFrameworkGenericRepositoryTests.Tests;

public class FindAllTests : BaseTest
{
    public FindAllTests(TestsContext context, IPersonRepository repository) : base(context, repository)
    {
    }

    [Theory]
    [MemberData(nameof(PeopleAndAmountDataGenerator))]
    public void When_FindAllCalled_Given_ListOfPeople_Then_CorrectAmountIsReturned(List<Person> data, int expectedAmount)
    {
        SetUpDatabase(data);

        ICollection<Person> people = Repository.FindAll();

        Assert.Equal(expectedAmount, people.Count);
    }
}