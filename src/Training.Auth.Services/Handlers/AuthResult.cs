namespace Training.Auth.Services.Handlers;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    string UserId,
    string Email,
    string Name,
    long ExpiresAt);
