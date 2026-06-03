using Microsoft.Extensions.Options;
using Training.Auth.Infrastructure.Services;

namespace Training.Auth.Tests.Services;

public class AuthTokenServiceTests
{
    private readonly AuthTokenService _service;

    public AuthTokenServiceTests()
    {
        var settings = new JwtSettings
        {
            SecretKey = "this-is-a-test-secret-key-that-is-long-enough-for-hmac",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        };

        _service = new AuthTokenService(Options.Create(settings));
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnTokenAndExpiry()
    {
        var (token, expiresAt) = _service.GenerateAccessToken(1, "test@test.com");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(expiresAt > DateTime.UtcNow);
        Assert.True(expiresAt < DateTime.UtcNow.AddHours(2));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnTokenAndExpiry()
    {
        var (token, expiresAt) = _service.GenerateRefreshToken();

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(expiresAt > DateTime.UtcNow.AddDays(6));
        Assert.True(expiresAt < DateTime.UtcNow.AddDays(8));
    }

    [Fact]
    public void ValidateAccessToken_ValidToken_ShouldReturnValid()
    {
        var (token, _) = _service.GenerateAccessToken(1, "test@test.com");

        var (isValid, isExpired, userId, email) = _service.ValidateAccessToken(token);

        Assert.True(isValid);
        Assert.False(isExpired);
        Assert.Equal(1, userId);
        Assert.Equal("test@test.com", email);
    }

    [Fact]
    public void ValidateAccessToken_InvalidToken_ShouldReturnInvalid()
    {
        var (isValid, isExpired, userId, email) = _service.ValidateAccessToken("invalid-token");

        Assert.False(isValid);
        Assert.False(isExpired);
        Assert.Equal(0, userId);
        Assert.Empty(email);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldBeUnique()
    {
        var (token1, _) = _service.GenerateRefreshToken();
        var (token2, _) = _service.GenerateRefreshToken();

        Assert.NotEqual(token1, token2);
    }
}
