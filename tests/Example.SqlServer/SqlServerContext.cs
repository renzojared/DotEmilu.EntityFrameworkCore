using DotEmilu.EntityFrameworkCore.Extensions;

namespace Example.SqlServer;

public class SqlServerContext(DbContextOptions<SqlServerContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .ApplyBaseAuditableEntityConfiguration(Assembly.GetExecutingAssembly())
            .ApplyBaseEntityConfiguration(Assembly.GetExecutingAssembly())
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Song> Songs => Set<Song>();
}