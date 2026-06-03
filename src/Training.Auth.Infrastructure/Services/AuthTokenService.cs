using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Training.Auth.Domain.Services;

namespace Training.Auth.Infrastructure.Services;

public class JwtSettings
{
    public string SecretKey { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class AuthTokenService : IAuthTokenService
{
    private readonly JwtSettings _settings;

    public AuthTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string token, DateTime expiresAt) GenerateAccessToken(string userId, string email)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_settings.SecretKey));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public (string token, DateTime expiresAt) GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return (Convert.ToBase64String(randomBytes), DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays));
    }

    public (bool isValid, bool isExpired, string userId, string email) ValidateAccessToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "";
            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "";

            var expClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim != null && long.TryParse(expClaim, out var expUnix))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                if (expDate < DateTimeOffset.UtcNow)
                    return (false, true, userId, email);
            }

            return (true, false, userId, email);
        }
        catch
        {
            return (false, false, "", "");
        }
    }
}
