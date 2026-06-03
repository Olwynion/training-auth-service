using Training.Auth.Grpc;
using Training.Auth.Infrastructure;
using Training.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapGrpcService<AuthGrpcService>();
app.MapGrpcReflectionService();
app.MapGet("/health", () => "OK");

app.Run();
