namespace Training.Auth.Domain.ValueObjects;

public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(string value) => new(Guid.Parse(value));
}
