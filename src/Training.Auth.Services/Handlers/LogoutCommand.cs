using MediatR;

namespace Training.Auth.Services.Handlers;

public record LogoutCommand(string UserId) : IRequest;
