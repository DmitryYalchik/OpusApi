using OpusApi.Repositories;

namespace OpusApi.Extensions;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddEntityRepositories(this IServiceCollection services)
    {
        var openInterface = typeof(IEntityRepository<>);

        var implementations = typeof(RepositoryRegistrationExtensions).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Select(t => new
            {
                Implementation = t,
                Interface = t.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openInterface)
            })
            .Where(x => x.Interface != null);

        foreach (var x in implementations)
        {
            services.AddScoped(x.Implementation);
            services.AddScoped(x.Interface!, sp => sp.GetRequiredService(x.Implementation));
        }

        return services;
    }
}
