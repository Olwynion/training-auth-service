using Dapper;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.ValueObjects;

namespace Training.Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM users WHERE id = @Id",
                parameters: new { Id = id.Value },
                cancellationToken: cancellationToken));

        return row != null ? MapToUser(row) : null;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM users WHERE email = @Email",
                parameters: new { Email = email.ToLowerInvariant().Trim() },
                cancellationToken: cancellationToken));

        return row != null ? MapToUser(row) : null;
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM users WHERE refresh_token = @RefreshToken",
                parameters: new { RefreshToken = refreshToken },
                cancellationToken: cancellationToken));

        return row != null ? MapToUser(row) : null;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO users (id, email, password_hash, name, refresh_token, refresh_token_expires_at, created_at, updated_at)
                    VALUES (@Id, @Email, @PasswordHash, @Name, @RefreshToken, @RefreshTokenExpiresAt, @CreatedAt, @UpdatedAt)",
                parameters: new
                {
                    Id = user.Id.Value,
                    user.Email,
                    user.PasswordHash,
                    user.Name,
                    user.RefreshToken,
                    user.RefreshTokenExpiresAt,
                    user.CreatedAt,
                    user.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public void Update(User user)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Execute(@"
            UPDATE users SET
                email = @Email, password_hash = @PasswordHash, name = @Name,
                refresh_token = @RefreshToken, refresh_token_expires_at = @RefreshTokenExpiresAt,
                updated_at = @UpdatedAt
            WHERE id = @Id",
            new
            {
                Id = user.Id.Value,
                user.Email,
                user.PasswordHash,
                user.Name,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.CreatedAt,
                user.UpdatedAt
            });
    }

    private static User MapToUser(dynamic row)
    {
        return User.Hydrate(
            UserId.From(((Guid)row.id).ToString()),
            (string)row.email,
            (string)row.password_hash,
            (string)row.name,
            (string?)row.refresh_token,
            (DateTime?)row.refresh_token_expires_at,
            (DateTime)row.created_at,
            (DateTime)row.updated_at);
    }
}
