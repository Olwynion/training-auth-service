using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Training.Auth.Services.Users.Commands;
using Training.Auth.Services.Users.Queries;
using static Training.Auth.AuthService;

namespace Training.Auth.Services;

public class AuthGrpcService(IMediator mediator) : AuthServiceBase
{
    public override async Task<AuthResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RegisterCommand(request.Email, request.Password, request.Name),
            context.CancellationToken);

        return ToAuthResponse(result);
    }

    public override async Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new LoginCommand(request.Email, request.Password),
            context.CancellationToken);

        return ToAuthResponse(result);
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

    public override async Task<AuthResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RefreshTokenCommand(request.RefreshToken),
            context.CancellationToken);

        return ToAuthResponse(result);
    }

    public override async Task<Empty> Logout(Training.Common.IdRequest request, ServerCallContext context)
    {
        await mediator.Send(new LogoutCommand(request.Id), context.CancellationToken);
        return new Empty();
    }

    private static AuthResponse ToAuthResponse(Training.Auth.Services.Users.DTOs.AuthResult result)
    {
        return new AuthResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Email = result.Email,
            Name = result.Name,
            ExpiresAt = result.ExpiresAt
        };
    }
}
