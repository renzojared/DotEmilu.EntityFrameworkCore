namespace DotEmilu.EntityFrameworkCore;

public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(s => s.IsDeleted)
            .IsRequired();

        builder
            .Property<byte[]>(nameof(Version))
            .IsRowVersion();

        builder
            .HasQueryFilter(s => !s.IsDeleted);
    }
}