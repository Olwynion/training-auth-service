using Training.Auth.Grpc;
using Training.Auth.Interceptors;

namespace Training.Auth;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });
        services.AddGrpcReflection();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
    }

    public void Configure(WebApplication app)
    {
        app.MapGrpcService<AuthGrpcService>();
        app.MapGrpcReflectionService();
        app.MapGet("/health", () => "OK");
    }
}
