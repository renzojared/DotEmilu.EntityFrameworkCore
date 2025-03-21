using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotEmilu.EntityFrameworkCore;

public static class DiContainer
{
    public static IServiceCollection AddSoftDeleteInterceptor(this IServiceCollection services)
        => services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

    public static IServiceCollection AddAuditableEntityInterceptor<TContextUser, TUserKey>(
        this IServiceCollection services)
        where TContextUser : class, IContextUser<TUserKey>
        where TUserKey : struct
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IContextUser<TUserKey>, TContextUser>();
        return services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor<TUserKey>>();
    }

    public static IServiceCollection AddAuditableEntityInterceptors(this IServiceCollection services, Assembly assembly)
    {
        services.TryAddSingleton(TimeProvider.System);

        var groupedByUserKeyType = GetContextUserImplementations(assembly)
            .Select(t => new
            {
                Implementation = t,
                Interface = t.GetInterfaces().First(IsContextUserInterface),
                UserKeyType = t.GetInterfaces().First(IsContextUserInterface).GetGenericArguments()[0]
            })
            .GroupBy(x => x.UserKeyType)
            .ToList();

        if (groupedByUserKeyType.Any(g => g.Count() > 1))
        {
            var duplicateKeys = string.Join(", ", groupedByUserKeyType
                .Where(g => g.Count() > 1)
                .Select(g => g.Key.Name));
            throw new ArgumentException(
                $"There are multiple implementations of IContextUser<T> for the following key types: {duplicateKeys}");
        }

        foreach (var group in groupedByUserKeyType)
        {
            var contextUser = group.First();
            services.TryAddScoped(contextUser.Interface, contextUser.Implementation);
            var userKeyType = group.Key;
            var auditableInterceptorType = typeof(AuditableEntityInterceptor<>).MakeGenericType(userKeyType);
            services.AddScoped(typeof(ISaveChangesInterceptor), auditableInterceptorType);
        }

        return services;

        static IEnumerable<Type> GetContextUserImplementations(Assembly assembly)
            => assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                            t.GetInterfaces().Any(IsContextUserInterface));

        static bool IsContextUserInterface(Type i)
            => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContextUser<>);
    }
}