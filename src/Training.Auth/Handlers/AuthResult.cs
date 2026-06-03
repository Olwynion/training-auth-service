namespace Training.Auth.Handlers;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    long UserId,
    string Email,
    string Name,
    long ExpiresAt);
