namespace Training.Auth.Services.Common;

public interface IAuthTokenService
{
    (string token, DateTime expiresAt) GenerateAccessToken(string userId, string email);
    (string token, DateTime expiresAt) GenerateRefreshToken();
    (bool isValid, bool isExpired, string userId, string email) ValidateAccessToken(string token);
}
