using MediatR;
using Training.Auth.Services.Users.DTOs;

namespace Training.Auth.Services.Users.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
