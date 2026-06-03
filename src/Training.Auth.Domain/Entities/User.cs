namespace Training.Auth.Domain.Entities;

public class User
{
    public long Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, string name)
    {
        return new User
        {
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static User Hydrate(
        long id, string email, string passwordHash, string name,
        string? refreshToken, DateTime? refreshTokenExpiresAt,
        DateTime createdAt, DateTime updatedAt)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Name = name,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void SetId(long id)
    {
        Id = id;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsRefreshTokenValid(string refreshToken)
    {
        return RefreshToken == refreshToken
            && RefreshTokenExpiresAt.HasValue
            && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
