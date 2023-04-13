using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class CountAllByIdTests : BaseIRepositoryTest
{
    public CountAllByIdTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(PeopleDataGenerator))]
    public void When_CountAllByIdCalled_Given_People_Then_ReturnCorrectAmount(List<Person> data)
    {
        SetUpDatabase(data);

        long amount = Repository.CountAllById(ALL_PEOPLE_IDS);

        Assert.Equal(data.Count, amount);
    }
}