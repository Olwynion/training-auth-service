using Grpc.Core;
using MediatR;
using Training.Auth;
using Training.Auth.Handlers;

namespace Training.Auth.Grpc;

public class AuthGrpcService(IMediator mediator) : AuthService.AuthServiceBase
{
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RegisterCommand(request.Email, request.Password, request.Name),
            context.CancellationToken);

        return new RegisterResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Email = result.Email,
            Name = result.Name,
            ExpiresAt = result.ExpiresAt
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new LoginCommand(request.Email, request.Password),
            context.CancellationToken);

        return new LoginResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Email = result.Email,
            Name = result.Name,
            ExpiresAt = result.ExpiresAt
        };
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new ValidateTokenQuery(request.AccessToken),
            context.CancellationToken);

        return new ValidateTokenResponse
        {
            IsValid = result.IsValid,
            IsExpired = result.IsExpired,
            UserId = result.UserId,
            Email = result.Email
        };
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RefreshTokenCommand(request.RefreshToken),
            context.CancellationToken);

        return new RefreshTokenResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Email = result.Email,
            Name = result.Name,
            ExpiresAt = result.ExpiresAt
        };
    }

    public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        await mediator.Send(new LogoutCommand(request.UserId), context.CancellationToken);
        return new LogoutResponse();
    }
}
