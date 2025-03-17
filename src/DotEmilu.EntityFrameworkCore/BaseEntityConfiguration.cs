namespace DotEmilu.EntityFrameworkCore;

public sealed class BaseEntityConfiguration<TBaseEntity, TKey>(MappingStrategy strategy, bool enableRowVersion)
    : IEntityTypeConfiguration<TBaseEntity>
    where TBaseEntity : class, IBaseEntity<TKey>
    where TKey : struct
{
    public void Configure(EntityTypeBuilder<TBaseEntity> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Id)
            .HasColumnOrder(0)
            .ValueGeneratedOnAdd();

        builder
            .Property(s => s.IsDeleted)
            .HasColumnOrder(1)
            .HasDefaultValue(false)
            .IsRequired();

        if (enableRowVersion)
            builder
                .Property<byte[]>(nameof(Version))
                .IsRowVersion()
                .IsRequired();

        builder
            .HasQueryFilter(s => !s.IsDeleted);

        builder
            .HasIndex(s => s.IsDeleted);

        builder
            .ApplyMappingStrategy(strategy);
    }
}