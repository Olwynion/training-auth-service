using Training.Auth.Infrastructure.Services;

namespace Training.Auth.Tests.Services;

public class BcryptPasswordHasherTests
{
    private readonly BcryptPasswordHasher _hasher;

    public BcryptPasswordHasherTests()
    {
        _hasher = new BcryptPasswordHasher();
    }

    [Fact]
    public void Hash_ShouldProduceDifferentHash()
    {
        var hash = _hasher.Hash("password123");

        Assert.NotNull(hash);
        Assert.NotEqual("password123", hash);
    }

    [Fact]
    public void Verify_CorrectPassword_ShouldReturnTrue()
    {
        var hash = _hasher.Hash("password123");

        Assert.True(_hasher.Verify("password123", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ShouldReturnFalse()
    {
        var hash = _hasher.Hash("password123");

        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_ShouldProduceUniqueHashes()
    {
        var hash1 = _hasher.Hash("same-password");
        var hash2 = _hasher.Hash("same-password");

        Assert.NotEqual(hash1, hash2);
    }
}
