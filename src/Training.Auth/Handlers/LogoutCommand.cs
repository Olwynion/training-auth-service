using MediatR;

namespace Training.Auth.Handlers;

public record LogoutCommand(long UserId) : IRequest;
