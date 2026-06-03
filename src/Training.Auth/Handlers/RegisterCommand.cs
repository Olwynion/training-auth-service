using MediatR;

namespace Training.Auth.Handlers;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<AuthResult>;
