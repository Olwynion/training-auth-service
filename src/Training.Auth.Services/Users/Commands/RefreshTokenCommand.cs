using MediatR;
using Training.Auth.Services.Users.DTOs;

namespace Training.Auth.Services.Users.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
