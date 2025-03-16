namespace Example.SqlServer.Entities;

public class Person : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? LastName { get; set; }
}

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder
            .Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(s => s.LastName)
            .HasMaxLength(100);
    }
}