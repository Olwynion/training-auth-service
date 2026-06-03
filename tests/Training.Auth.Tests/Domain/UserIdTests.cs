using Training.Auth.Domain.ValueObjects;

namespace Training.Auth.Tests.Domain;

public class UserIdTests
{
    [Fact]
    public void New_ShouldGenerateNonEmpty()
    {
        var id = UserId.New();

        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_ShouldParseGuid()
    {
        var guid = Guid.NewGuid();
        var id = UserId.From(guid.ToString());

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void Equality_ShouldWork()
    {
        var guid = Guid.NewGuid();
        var id1 = UserId.From(guid.ToString());
        var id2 = UserId.From(guid.ToString());

        Assert.Equal(id1, id2);
    }
}
