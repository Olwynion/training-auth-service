using MediatR;

namespace Training.Auth.Handlers;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
