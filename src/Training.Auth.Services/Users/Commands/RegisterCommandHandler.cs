using MediatR;
using Training.Auth.Services.Common;
using Training.Auth.Services.Users.DTOs;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;

namespace Training.Auth.Services.Users.Commands;

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

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(
            user.Id.Value.ToString(), user.Email);
        var (refreshToken, _) = tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, expiresAt);

        await userRepository.AddAsync(user, cancellationToken);

        return new AuthResult(
            accessToken, refreshToken,
            user.Id.Value.ToString(), user.Email, user.Name,
            ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
    }
}
