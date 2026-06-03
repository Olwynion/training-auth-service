using Training.Auth.Domain.Entities;
using Training.Auth.Domain.ValueObjects;

namespace Training.Auth.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var user = User.Create("test@example.com", "hash123", "Test User");

        Assert.NotNull(user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal("Test User", user.Name);
    }

    [Fact]
    public void Create_ShouldNormalizeEmailToLower()
    {
        var user = User.Create("TEST@Example.COM", "hash", "User");

        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void Create_ShouldSetTimestamps()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var user = User.Create("a@b.com", "hash", "Name");

        Assert.InRange(user.CreatedAt, before, DateTime.UtcNow.AddSeconds(1));
        Assert.InRange(user.UpdatedAt, before, DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Hydrate_ShouldReconstructUser()
    {
        var id = UserId.New();
        var now = DateTime.UtcNow;

        var user = User.Hydrate(
            id, "test@test.com", "hash", "Name",
            "refresh", now.AddDays(1), now, now);

        Assert.Equal(id, user.Id);
        Assert.Equal("test@test.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal("Name", user.Name);
        Assert.Equal("refresh", user.RefreshToken);
        Assert.Equal(now.AddDays(1), user.RefreshTokenExpiresAt);
        Assert.Equal(now, user.CreatedAt);
    }

    [Fact]
    public void SetRefreshToken_ShouldUpdateTokenAndExpiry()
    {
        var user = User.Create("a@b.com", "hash", "N");
        var expires = DateTime.UtcNow.AddDays(7);

        user.SetRefreshToken("new-token", expires);

        Assert.Equal("new-token", user.RefreshToken);
        Assert.Equal(expires, user.RefreshTokenExpiresAt);
    }

    [Fact]
    public void IsRefreshTokenValid_WithValidToken_ShouldReturnTrue()
    {
        var user = User.Create("a@b.com", "hash", "N");
        user.SetRefreshToken("valid-token", DateTime.UtcNow.AddDays(1));

        Assert.True(user.IsRefreshTokenValid("valid-token"));
    }

    [Fact]
    public void IsRefreshTokenValid_WithExpiredToken_ShouldReturnFalse()
    {
        var user = User.Create("a@b.com", "hash", "N");
        user.SetRefreshToken("expired-token", DateTime.UtcNow.AddDays(-1));

        Assert.False(user.IsRefreshTokenValid("expired-token"));
    }

    [Fact]
    public void IsRefreshTokenValid_WithWrongToken_ShouldReturnFalse()
    {
        var user = User.Create("a@b.com", "hash", "N");
        user.SetRefreshToken("real-token", DateTime.UtcNow.AddDays(1));

        Assert.False(user.IsRefreshTokenValid("wrong-token"));
    }

    [Fact]
    public void ClearRefreshToken_ShouldRemoveToken()
    {
        var user = User.Create("a@b.com", "hash", "N");
        user.SetRefreshToken("token", DateTime.UtcNow.AddDays(1));

        user.ClearRefreshToken();

        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiresAt);
    }
}
