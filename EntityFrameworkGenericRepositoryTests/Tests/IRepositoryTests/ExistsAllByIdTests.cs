using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class ExistsAllByIdTests : BaseIRepositoryTest
{
    public ExistsAllByIdTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(NonEmptyPeopleDataGenerator))]
    public void When_ExistsByIdCalled_Given_AnExistingPersonId_Then_ReturnTrue(List<Person> data)
    {
        SetUpDatabase(data);

        List<int> ids = data.Select(person => person.Id).ToList();

        foreach (int id in ids)
        {
            bool exists = Repository.ExistsById(id);
            Assert.True(exists);
        }
    }

    [Theory]
    [MemberData(nameof(EmptyPeopleDataGenerator))]
    public void When_ExistsByIdCalled_Given_ANonExistingPersonId_Then_ReturnFalse(List<Person> data)
    {
        SetUpDatabase(data);

        foreach (int id in ALL_PEOPLE_IDS)
        {
            bool exists = Repository.ExistsById(id);
            Assert.False(exists);
        }
    }
}