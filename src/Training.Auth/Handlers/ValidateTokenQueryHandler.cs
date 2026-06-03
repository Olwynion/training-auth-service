using MediatR;
using Training.Auth.Domain.Services;

namespace Training.Auth.Handlers;

public class ValidateTokenQueryHandler(IAuthTokenService tokenService)
    : IRequestHandler<ValidateTokenQuery, TokenValidationResult>
{
    public Task<TokenValidationResult> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var (isValid, isExpired, userId, email) = tokenService.ValidateAccessToken(request.AccessToken);
        return Task.FromResult(new TokenValidationResult(isValid, isExpired, userId, email));
    }
}
