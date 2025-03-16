namespace DotEmilu.EntityFrameworkCore;

public class BaseEntityConfiguration(MappingStrategy strategy = MappingStrategy.Tpc, bool useRowVersion = false)
    : IEntityTypeConfiguration<BaseEntity>
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
            .HasDefaultValue(false)
            .IsRequired();

        if (useRowVersion)
            builder
                .Property<byte[]>(nameof(Version))
                .IsRowVersion()
                .IsRequired();

        builder
            .HasQueryFilter(s => !s.IsDeleted);

        builder
            .HasIndex(s => s.IsDeleted);

        switch (strategy)
        {
            case MappingStrategy.Tph:
                builder.UseTphMappingStrategy();
                break;
            case MappingStrategy.Tpt:
                builder.UseTptMappingStrategy();
                break;
            case MappingStrategy.Tpc:
            default:
                builder.UseTpcMappingStrategy();
                break;
        }
    }
}