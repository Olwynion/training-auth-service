using Moq;
using Training.Auth.Domain.Entities;
using Training.Auth.Domain.Repositories;
using Training.Auth.Domain.Services;
using Training.Auth.Handlers;

namespace Training.Auth.Tests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IAuthTokenService> _tokenService;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _tokenService = new Mock<IAuthTokenService>();
        _passwordHasher = new Mock<IPasswordHasher>();

        _handler = new LoginCommandHandler(
            _userRepo.Object, _tokenService.Object, _passwordHasher.Object);

        _tokenService.Setup(t => t.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(("access-token", DateTime.UtcNow.AddHours(1)));
        _tokenService.Setup(t => t.GenerateRefreshToken())
            .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResult()
    {
        var user = User.Create("test@test.com", "correct-hash", "Test");
        _userRepo.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(p => p.Verify("correct-pass", "correct-hash")).Returns(true);

        var result = await _handler.Handle(
            new LoginCommand("test@test.com", "correct-pass"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldThrow()
    {
        var user = User.Create("test@test.com", "correct-hash", "Test");
        _userRepo.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(p => p.Verify("wrong-pass", "correct-hash")).Returns(false);

        var command = new LoginCommand("test@test.com", "wrong-pass");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ShouldThrow()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("unknown@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("unknown@test.com", "pass");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
