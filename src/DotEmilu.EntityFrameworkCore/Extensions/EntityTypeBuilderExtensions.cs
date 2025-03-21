namespace DotEmilu.EntityFrameworkCore.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void ApplyMappingStrategy<T>(this EntityTypeBuilder<T> builder, MappingStrategy strategy)
        where T : class
    {
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

    public static void UseIsDeleted<T>(this EntityTypeBuilder<T> builder, int order = 0)
        where T : class, IBaseEntity
    {
        builder
            .Property(s => s.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        if (order != 0)
            builder
                .Property(s => s.IsDeleted)
                .HasColumnOrder(order);

        builder
            .HasQueryFilter(s => !s.IsDeleted);

        builder
            .HasIndex(s => s.IsDeleted);
    }
}