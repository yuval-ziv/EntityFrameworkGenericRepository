using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using static EntityFrameworkGenericRepositoryTests.DataGenerators.PeopleData;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public class FindByIdTests : BaseIRepositoryTest
{
    public FindByIdTests(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Theory]
    [MemberData(nameof(NonEmptyPeopleDataGenerator))]
    public void When_FindByIdCalled_Given_AnExistingPersonId_Then_PersonIsNotNull(List<Person> data)
    {
        SetUpDatabase(data);

        List<int> ids = data.Select(person => person.Id).ToList();

        foreach (Person? person in ids.Select(id => Repository.FindById(id)))
        {
            Assert.NotNull(person);
        }
    }

    [Theory]
    [MemberData(nameof(EmptyPeopleDataGenerator))]
    public void When_FindByIdCalled_Given_ANonExistingPersonId_Then_PersonIsNull(List<Person> data)
    {
        SetUpDatabase(data);

        foreach (Person? person in ALL_PEOPLE_IDS.Select(id => Repository.FindById(id)))
        {
            Assert.Null(person);
        }
    }
}