using Moq;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;
using Training.Auth.Handlers;

namespace Training.Auth.Tests.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IAuthTokenService> _tokenService;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _tokenService = new Mock<IAuthTokenService>();
        _passwordHasher = new Mock<IPasswordHasher>();

        _handler = new RegisterCommandHandler(
            _userRepo.Object, _tokenService.Object, _passwordHasher.Object);

        _passwordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed-password");
        _tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddHours(1)));
        _tokenService.Setup(t => t.GenerateRefreshToken())
            .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public async Task Handle_WhenEmailNotTaken_ShouldCreateUser()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new RegisterCommand("new@test.com", "pass123", "New User"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal("new@test.com", result.Email);
        Assert.Equal("New User", result.Name);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ShouldThrow()
    {
        var existing = User.Create("existing@test.com", "hash", "Existing");
        _userRepo.Setup(r => r.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new RegisterCommand("existing@test.com", "pass", "User");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
