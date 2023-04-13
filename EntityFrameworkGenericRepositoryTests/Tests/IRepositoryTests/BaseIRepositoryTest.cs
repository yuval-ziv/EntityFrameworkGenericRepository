using EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;
using EntityFrameworkGenericRepositoryTests.Tests.Abstraction;

namespace EntityFrameworkGenericRepositoryTests.Tests.IRepositoryTests;

public abstract class BaseIRepositoryTest : BaseTest
{
    protected readonly IPersonRepository Repository;

    protected BaseIRepositoryTest(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Repository = new PersonRepository(TestsContextFactory);
    }
}