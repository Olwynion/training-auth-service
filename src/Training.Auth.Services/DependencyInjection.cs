using Microsoft.Extensions.DependencyInjection;

namespace Training.Auth.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
