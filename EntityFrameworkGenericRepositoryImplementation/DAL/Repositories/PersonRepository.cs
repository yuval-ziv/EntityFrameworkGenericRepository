using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public class PersonRepository : BaseRepository<Person, int, TestsContext>, IPersonRepository
{
    public PersonRepository(IDbContextFactory<TestsContext> contextFactory) : base(contextFactory)
    {
    }
}