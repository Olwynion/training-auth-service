namespace Training.Auth.Domain.Services;

public interface IAuthTokenService
{
    (string token, DateTime expiresAt) GenerateAccessToken(long userId, string email);
    (string token, DateTime expiresAt) GenerateRefreshToken();
    (bool isValid, bool isExpired, long userId, string email) ValidateAccessToken(string token);
}
