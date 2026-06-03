using MediatR;
using Training.Auth.Services.Common;

namespace Training.Auth.Services.Users.Queries;

public class ValidateTokenQueryHandler(IAuthTokenService tokenService)
    : IRequestHandler<ValidateTokenQuery, TokenValidationResult>
{
    public Task<TokenValidationResult> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var (isValid, isExpired, userId, email) = tokenService.ValidateAccessToken(request.AccessToken);
        return Task.FromResult(new TokenValidationResult(isValid, isExpired, userId, email));
    }
}
