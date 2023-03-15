using EntityFrameworkGenericRepositoryImplementation.DAL.Context;
using EntityFrameworkGenericRepositoryImplementation.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkGenericRepositoryTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IPersonRepository, PersonRepository>();

        services.AddDbContextFactory<TestsContext>(options =>
        {
            options.UseSqlite("Data Source=.//test.sqlite");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }
}