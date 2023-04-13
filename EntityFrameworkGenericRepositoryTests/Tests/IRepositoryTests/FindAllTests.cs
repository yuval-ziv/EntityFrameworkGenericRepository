using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class FindAllTests : BaseIRepositoryTest
{
    public FindAllTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(PeopleDataGenerator))]
    public void When_FindAllCalled_Given_ListOfPeople_Then_CorrectAmountIsReturned(List<Person> data)
    {
        SetUpDatabase(data);

        ICollection<Person> people = Repository.FindAll();

        Assert.Equal(data.Count, people.Count);
    }
}