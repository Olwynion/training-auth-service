using MediatR;

namespace Training.Auth.Handlers;

public record LogoutCommand(string UserId) : IRequest;
