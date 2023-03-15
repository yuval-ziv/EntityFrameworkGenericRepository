using EntityFrameworkGenericRepositoryImplementation.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Context;

public class TestsContext : DbContext
{
    public DbSet<Person> People { get; set; }

    protected TestsContext()
    {
        Initialize();
    }

    public TestsContext(DbContextOptions options) : base(options)
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Database.IsSqlite())
        {
            Database.EnsureCreated();
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // builder.Entity<Person>().HasOne(person => person.Parent).WithMany(person => person.Children);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (options.IsConfigured)
        {
            return;
        }

        options.UseSqlite("Data Source=.\\test.sqlite");
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}