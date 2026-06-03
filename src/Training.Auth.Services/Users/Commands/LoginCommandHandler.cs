using MediatR;
using Training.Auth.Services.Common;
using Training.Auth.Services.Users.DTOs;
using Training.Auth.Domain.Repositories;

namespace Training.Auth.Services.Users.Commands;

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
