using MediatR;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;

namespace Training.Auth.Handlers;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IAuthTokenService tokenService,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginCommand, AuthResult>
{
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user.Id, user.Email);
        var (refreshToken, _) = tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, expiresAt);
        userRepository.Update(user);

        return new AuthResult(
            accessToken, refreshToken,
            user.Id, user.Email, user.Name,
            ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
    }
}
