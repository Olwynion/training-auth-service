using MediatR;

namespace Training.Auth.Services.Users.Commands;

public record LogoutCommand(string UserId) : IRequest;
