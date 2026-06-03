using Moq;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;
using Training.Auth.Handlers;

namespace Training.Auth.Tests.Handlers;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IAuthTokenService> _tokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _tokenService = new Mock<IAuthTokenService>();
        _handler = new RefreshTokenCommandHandler(_userRepo.Object, _tokenService.Object);

        _tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(("new-access-token", DateTime.UtcNow.AddHours(1)));
        _tokenService.Setup(t => t.GenerateRefreshToken())
            .Returns(("new-refresh-token", DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        var user = User.Create("test@test.com", "hash", "Test");
        user.SetRefreshToken("valid-refresh", DateTime.UtcNow.AddDays(1));
        _userRepo.Setup(r => r.GetByRefreshTokenAsync("valid-refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(
            new RefreshTokenCommand("valid-refresh"),
            CancellationToken.None);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ShouldThrow()
    {
        _userRepo.Setup(r => r.GetByRefreshTokenAsync("bad-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new RefreshTokenCommand("bad-token");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
