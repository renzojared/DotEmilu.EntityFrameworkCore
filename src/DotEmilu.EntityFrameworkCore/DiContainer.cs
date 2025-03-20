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
}