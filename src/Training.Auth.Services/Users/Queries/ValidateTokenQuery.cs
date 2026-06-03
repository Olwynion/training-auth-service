using MediatR;

namespace Training.Auth.Services.Users.Queries;

public record ValidateTokenQuery(string AccessToken) : IRequest<TokenValidationResult>;

public record TokenValidationResult(bool IsValid, bool IsExpired, string UserId, string Email);
