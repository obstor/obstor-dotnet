using Obstor.IntegrationTests.Helpers;
using Testcontainers.Obstor;
using Xunit;

namespace Obstor.IntegrationTests.Tests;

public abstract class ObstorTest : IAsyncLifetime
{
    private readonly ObstorContainer _obstorContainer = new ObstorBuilder(ImageConstants.Obstor).Build();

    public Task InitializeAsync() => _obstorContainer.StartAsync();
    public Task DisposeAsync() => _obstorContainer.StopAsync();

    protected IObstorClient CreateClient()
    {
        return new ObstorClientBuilder(_obstorContainer.GetConnectionString())
            .WithStaticCredentials(_obstorContainer.GetAccessKey(), _obstorContainer.GetSecretKey())
            .Build();
    }
}