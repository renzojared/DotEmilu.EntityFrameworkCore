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
            .UseIsDeleted(order: 1);

        if (enableRowVersion)
            builder
                .Property<byte[]>(nameof(Version))
                .IsRowVersion()
                .IsRequired();

        builder
            .ApplyMappingStrategy(strategy);
    }
}