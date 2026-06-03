using MediatR;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;

namespace Training.Auth.Handlers;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IAuthTokenService tokenService) : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (user == null || !user.IsRefreshTokenValid(request.RefreshToken))
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(
            user.Id.Value.ToString(), user.Email);
        var (refreshToken, _) = tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, expiresAt);
        userRepository.Update(user);

        return new AuthResult(
            accessToken, refreshToken,
            user.Id.Value.ToString(), user.Email, user.Name,
            ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
    }
}
