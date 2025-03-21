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
        Assembly assembly,
        Dictionary<Type, (MappingStrategy strategy, bool enableRowVersion)>? keyTypeConfigurations = null)
    {
        var entityTypes = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        t.GetInterfaces().Any(IsBaseEntityInterface));

        foreach (var entityType in entityTypes)
        {
            var keyType = entityType.GetInterfaces()
                .First(IsBaseEntityInterface)
                .GetGenericArguments()[0];

            var (strategy, enableRowVersion) =
                keyTypeConfigurations?.TryGetValue(keyType, out var configuration) is true
                    ? configuration
                    : (MappingStrategy.Tpc, false);

            var configType = typeof(BaseEntityConfiguration<,>).MakeGenericType(entityType, keyType);
            var configInstance = Activator.CreateInstance(configType, strategy, enableRowVersion);
            modelBuilder.ApplyConfiguration((dynamic)configInstance!);
        }

        return modelBuilder;

        static bool IsBaseEntityInterface(Type i)
            => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseEntity<>);
    }

    [Obsolete("Use ApplyBaseEntityConfiguration(Assembly) instead.", true)]
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
        Assembly assembly,
        Dictionary<Type, MappingStrategy>? userKeyConfigurations = null)
    {
        var entityTypes = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        t.GetInterfaces().Any(IsBaseAuditableEntityInterface));

        foreach (var entityType in entityTypes)
        {
            var userKeyType = entityType.GetInterfaces()
                .First(IsBaseAuditableEntityInterface)
                .GetGenericArguments()[0];

            var strategy = userKeyConfigurations?.TryGetValue(userKeyType, out var config) is true
                ? config
                : MappingStrategy.Tpc;

            var configType = typeof(BaseAuditableEntityConfiguration<,>).MakeGenericType(entityType, userKeyType);
            var configInstance = Activator.CreateInstance(configType, strategy);
            modelBuilder.ApplyConfiguration((dynamic)configInstance!);
        }

        return modelBuilder;

        static bool IsBaseAuditableEntityInterface(Type i)
            => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseAuditableEntity<>);
    }

    [Obsolete("Use ApplyBaseAuditableEntityConfiguration(Assembly) instead.", true)]
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