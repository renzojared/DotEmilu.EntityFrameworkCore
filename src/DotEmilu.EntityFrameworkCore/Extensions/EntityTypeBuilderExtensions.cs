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
}