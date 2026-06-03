using MediatR;

namespace Training.Auth.Services.Handlers;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<AuthResult>;
