using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public interface IPersonAsyncRepository : IAsyncRepository<Person, int>
{
}