using MediatR;
using Training.Auth.Services.Users.DTOs;

namespace Training.Auth.Services.Users.Commands;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<AuthResult>;
