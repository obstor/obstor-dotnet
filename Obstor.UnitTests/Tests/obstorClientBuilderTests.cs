using Xunit;

namespace Obstor.UnitTests.Tests;

public class ObstorClientBuilderTests
{
    [Fact]
    public void EnsureObstorClient()
    {
        var obstorClient = new ObstorClientBuilder("http://localhost:9000")
            .WithStaticCredentials("obstor", "obstor123")
            .Build();
        Assert.NotNull(obstorClient);
    }

    [Fact]
    public void EnsureExceptionWithoutCredentialsProvider()
    {
        Assert.Throws<InvalidOperationException>(() => new ObstorClientBuilder("http://localhost:9000").Build());
    }
}