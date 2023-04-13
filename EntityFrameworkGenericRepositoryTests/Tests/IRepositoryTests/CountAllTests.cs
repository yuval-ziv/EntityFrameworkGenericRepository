using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class CountAllTests : BaseIRepositoryTest
{
    public CountAllTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(PeopleDataGenerator))]
    public void When_CountAllCalled_Given_People_Then_ReturnCorrectAmount(List<Person> data)
    {
        SetUpDatabase(data);

        long amount = Repository.CountAll();

        Assert.Equal(data.Count, amount);
    }
}