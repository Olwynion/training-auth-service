using Training.Auth.Grpc;
using Training.Auth.Infrastructure;
using Training.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapGrpcService<AuthGrpcService>();
app.MapGet("/health", () => "OK");

app.Run();
