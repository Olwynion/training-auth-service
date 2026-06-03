using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;
using Training.Auth.Infrastructure.Factories;
using Training.Auth.Infrastructure.Repositories;
using Training.Auth.Infrastructure.Services;

namespace Training.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
