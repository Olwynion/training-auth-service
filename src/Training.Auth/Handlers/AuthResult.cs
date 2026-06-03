namespace Training.Auth.Handlers;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    string UserId,
    string Email,
    string Name,
    long ExpiresAt);
