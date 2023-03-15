using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public interface IPersonAsyncPagedRepository : IAsyncPagedRepository<Person, int,PersonFilter>
{
}