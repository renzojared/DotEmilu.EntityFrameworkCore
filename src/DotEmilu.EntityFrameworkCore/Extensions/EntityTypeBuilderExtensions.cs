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

    /// <summary>
    /// Configures the IsDeleted property for soft delete functionality
    /// </summary>
    /// <typeparam name="T">Entity type that implements IBaseEntity</typeparam>
    /// <param name="builder">Entity type builder</param>
    /// <param name="useShort">If true, stores as short (0/1) instead of boolean</param>
    /// <param name="order">Column order (0 = no specific order)</param>
    /// <param name="useIndex">Whether to create an index on IsDeleted column</param>
    /// <param name="useQueryFilter">Whether to add a global query filter to exclude deleted records</param>
    /// <returns>Entity type builder for chaining</returns>
    public static EntityTypeBuilder<T> UseIsDeleted<T>(this EntityTypeBuilder<T> builder,
        bool useShort = false,
        bool useIndex = true,
        bool useQueryFilter = true,
        int order = 0) where T : class, IBaseEntity
    {
        var propertyBuilder = builder.Property(s => s.IsDeleted);

        if (useShort)
        {
            propertyBuilder
                .HasDefaultValue(0)
                .HasShortConversion()
                .HasComment("Soft delete: 1 is deleted")
                .IsRequired();
        }
        else
        {
            propertyBuilder
                .HasDefaultValue(false)
                .HasComment("Soft delete: true is deleted")
                .IsRequired();
        }

        if (order > 0)
            propertyBuilder.HasColumnOrder(order);

        if (useQueryFilter)
            builder.HasQueryFilter(s => !s.IsDeleted);

        if (useIndex)
            builder.HasIndex(s => s.IsDeleted);

        return builder;
    }

    /// <summary>
    /// Converts boolean to short integer (0/1) in a database.
    /// Recommended: Use properties starting with 'Is' (e.g., IsActive, IsDeleted, IsDisabled) 
    /// so that 1 always represents the true/positive state.
    /// </summary>
    public static PropertyBuilder<bool> HasShortConversion(this PropertyBuilder<bool> propertyBuilder)
        => propertyBuilder.HasConversion(
            convertToProviderExpression: b => b ? (short)1 : (short)0,
            convertFromProviderExpression: value => value == 1);
}