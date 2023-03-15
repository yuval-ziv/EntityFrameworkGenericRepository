using EntityFrameworkGenericRepository.Repositories;
using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;

public interface IPersonPagedRepository : IPagedRepository<Person, int,PersonFilter>
{
}