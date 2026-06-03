using Grpc.Core;
using MediatR;
using Moq;
using Training.Auth.Grpc;
using Training.Auth.Handlers;

namespace Training.Auth.Tests.GrpcServices;

public class AuthGrpcServiceTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly AuthGrpcService _service;

    public AuthGrpcServiceTests()
    {
        _mediator = new Mock<IMediator>();
        _service = new AuthGrpcService(_mediator.Object);
    }

    [Fact]
    public async Task Register_ShouldSendRegisterCommand()
    {
        _mediator.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResult("at", "rt", 99, "e@m.com", "n", 12345L));

        var response = await _service.Register(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "pass",
            Name = "Test"
        }, Mock.Of<ServerCallContext>());

        Assert.Equal("at", response.AccessToken);
        Assert.Equal(99, response.UserId);
    }

    [Fact]
    public async Task Login_ShouldSendLoginCommand()
    {
        _mediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResult("at", "rt", 99, "e@m.com", "n", 12345L));

        var response = await _service.Login(new LoginRequest
        {
            Email = "test@test.com",
            Password = "pass"
        }, Mock.Of<ServerCallContext>());

        Assert.Equal("at", response.AccessToken);
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnValidationResult()
    {
        _mediator.Setup(m => m.Send(It.IsAny<ValidateTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenValidationResult(true, false, 99, "e@m.com"));

        var response = await _service.ValidateToken(new ValidateTokenRequest
        {
            AccessToken = "token"
        }, Mock.Of<ServerCallContext>());

        Assert.True(response.IsValid);
        Assert.Equal(99, response.UserId);
    }

    [Fact]
    public async Task RefreshToken_ShouldSendRefreshTokenCommand()
    {
        _mediator.Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResult("new-at", "new-rt", 99, "e@m.com", "n", 12345L));

        var response = await _service.RefreshToken(new RefreshTokenRequest
        {
            RefreshToken = "rt"
        }, Mock.Of<ServerCallContext>());

        Assert.Equal("new-at", response.AccessToken);
        Assert.Equal("new-rt", response.RefreshToken);
    }

    [Fact]
    public async Task Logout_ShouldSendLogoutCommand()
    {
        _mediator.Setup(m => m.Send(It.IsAny<LogoutCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _service.Logout(new LogoutRequest
        {
            UserId = 99
        }, Mock.Of<ServerCallContext>());

        Assert.NotNull(response);
    }
}
