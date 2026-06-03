using Training.Auth.Domain.Entities;

namespace Training.Auth.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<long> AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}
