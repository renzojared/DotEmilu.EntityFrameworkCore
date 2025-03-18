namespace Example.SqlServer.Entities;

public class Song : BaseAuditableEntity<Guid>
{
    public required string Name { get; set; }
}

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder
            .Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}