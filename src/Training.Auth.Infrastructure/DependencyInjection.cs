using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;
using Training.Auth.Infrastructure.Mappings;
using Training.Auth.Infrastructure.Repositories;
using Training.Auth.Infrastructure.Services;

namespace Training.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new UserIdTypeHandler());

        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();

        return services;
    }
}
