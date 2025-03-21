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

    public static ModelBuilder ApplyBaseEntityConfiguration(this ModelBuilder modelBuilder,
        MappingStrategy strategy = MappingStrategy.Tpc, bool enableRowVersion = false, params Type[] keyTypes)
    {
        if (keyTypes.Any(type => !type.IsValueType))
            throw new ArgumentException("All keyTypes must be value types (structs).", nameof(keyTypes));

        foreach (var keyType in keyTypes)
        {
            var baseEntityGenericType = typeof(IBaseEntity<>).MakeGenericType(keyType);

            foreach (var entityType in modelBuilder.Model
                         .GetEntityTypes()
                         .Where(e => baseEntityGenericType
                             .IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract))
            {
                var configType = typeof(BaseEntityConfiguration<,>)
                    .MakeGenericType(entityType.ClrType, keyType);
                var configInstance = Activator.CreateInstance(configType, strategy, enableRowVersion);
                modelBuilder.ApplyConfiguration((dynamic)configInstance!);
            }
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

    public static ModelBuilder ApplyBaseAuditableEntityConfiguration(this ModelBuilder modelBuilder,
        MappingStrategy strategy = MappingStrategy.Tpc, params Type[] userKeyTypes)
    {
        if (userKeyTypes.Any(type => !type.IsValueType))
            throw new ArgumentException("All userKeyTypes must be value types (structs).", nameof(userKeyTypes));

        foreach (var userKeyType in userKeyTypes)
        {
            var baseAuditableEntityGenericType = typeof(IBaseAuditableEntity<>).MakeGenericType(userKeyType);

            foreach (var entityType in modelBuilder.Model
                         .GetEntityTypes()
                         .Where(e => baseAuditableEntityGenericType
                             .IsAssignableFrom(e.ClrType) && !e.ClrType.IsAbstract))
            {
                var configType = typeof(BaseAuditableEntityConfiguration<,>)
                    .MakeGenericType(entityType.ClrType, userKeyType);
                var configInstance = Activator.CreateInstance(configType, strategy);
                modelBuilder.ApplyConfiguration((dynamic)configInstance!);
            }
        }

        return modelBuilder;
    }
}