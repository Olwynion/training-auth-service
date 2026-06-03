using MediatR;

namespace Training.Auth.Handlers;

public record ValidateTokenQuery(string AccessToken) : IRequest<TokenValidationResult>;

public record TokenValidationResult(bool IsValid, bool IsExpired, string UserId, string Email);
