using MediatR;

namespace Training.Auth.Handlers;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
