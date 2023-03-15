using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public class PersonAsyncRepository : BaseAsyncRepository<Person, int, TestsContext>, IPersonAsyncRepository
{
    public PersonAsyncRepository(IDbContextFactory<TestsContext> contextFactory) : base(contextFactory)
    {
    }
}