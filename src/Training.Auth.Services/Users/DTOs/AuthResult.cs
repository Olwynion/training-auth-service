namespace Training.Auth.Services.Users.DTOs;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    string UserId,
    string Email,
    string Name,
    long ExpiresAt);
