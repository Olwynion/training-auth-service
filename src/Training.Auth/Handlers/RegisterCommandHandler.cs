using MediatR;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;

namespace Training.Auth.Handlers;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IAuthTokenService tokenService,
    IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("User with this email already exists");

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.Name);

        var userId = await userRepository.AddAsync(user, cancellationToken);
        user.SetId(userId);

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(userId, user.Email);
        var (refreshToken, _) = tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, expiresAt);
        userRepository.Update(user);

        return new AuthResult(
            accessToken, refreshToken,
            userId, user.Email, user.Name,
            ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
    }
}
