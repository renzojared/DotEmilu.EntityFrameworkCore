namespace DotEmilu.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyBaseEntityConfiguration<TKey>(this ModelBuilder modelBuilder,
        MappingStrategy strategy = MappingStrategy.Tpc, bool enableRowVersion = false)
        where TKey : struct
    {
        foreach (var entityType in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(e => typeof(IBaseEntity<TKey>)
                         .IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract))
        {
            var configType = typeof(BaseEntityConfiguration<,>)
                .MakeGenericType(entityType.ClrType, typeof(TKey));
            var configInstance = Activator.CreateInstance(configType, strategy, enableRowVersion);
            modelBuilder.ApplyConfiguration((dynamic)configInstance!);
        }

        return modelBuilder;
    }

    public static ModelBuilder ApplyBaseAuditableEntityConfiguration<TUserKey>(this ModelBuilder modelBuilder,
        MappingStrategy strategy = MappingStrategy.Tpc)
        where TUserKey : struct
    {
        foreach (var entityType in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(e => typeof(IBaseAuditableEntity<TUserKey>)
                         .IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract))
        {
            var configType = typeof(BaseAuditableEntityConfiguration<,>)
                .MakeGenericType(entityType.ClrType, typeof(TUserKey));
            var configInstance = Activator.CreateInstance(configType, strategy);
            modelBuilder.ApplyConfiguration((dynamic)configInstance!);
        }

        return modelBuilder;
    }
}