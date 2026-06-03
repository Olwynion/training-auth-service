using Training.Auth.Infrastructure;
using Training.Auth.Services;
using Training.Auth.Services.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<AuthGrpcService>();
app.MapGet("/health", () => "OK");

app.Run();
