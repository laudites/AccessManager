using Microsoft.Extensions.DependencyInjection;

namespace AccessManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
