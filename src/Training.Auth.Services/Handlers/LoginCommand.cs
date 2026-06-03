using MediatR;

namespace Training.Auth.Services.Handlers;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
