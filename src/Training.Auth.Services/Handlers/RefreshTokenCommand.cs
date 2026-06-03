using MediatR;

namespace Training.Auth.Services.Handlers;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
