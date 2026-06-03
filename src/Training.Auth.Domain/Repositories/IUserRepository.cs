using Training.Auth.Domain.Entities;
using Training.Auth.Domain.ValueObjects;

namespace Training.Auth.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}
