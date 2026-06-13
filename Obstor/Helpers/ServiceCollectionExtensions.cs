using Microsoft.Extensions.DependencyInjection.Extensions;
using Obstor;
using Obstor.CredentialProviders;
using Obstor.Implementation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A fluent builder interface returned by <see cref="ServiceCollectionServiceExtensions.AddObstor(IServiceCollection, Action{Obstor.ClientOptions}?, ServiceLifetime)"/>
/// that allows callers to configure the credential provider used by the Obstor client registered in the
/// dependency injection container.
/// </summary>
public interface IObstorBuilder
{
    /// <summary>
    /// Configures the Obstor client to authenticate using static (hardcoded) credentials,
    /// applying the supplied delegate to an <see cref="StaticCredentialsOptions"/> instance.
    /// </summary>
    /// <param name="configure">A delegate that sets access key, secret key, and optional session token on the options object.</param>
    /// <returns>The current <see cref="IObstorBuilder"/> instance to allow further chaining.</returns>
    IObstorBuilder WithStaticCredentials(Action<StaticCredentialsOptions> configure);

    /// <summary>
    /// Configures the Obstor client to authenticate using the supplied static credentials.
    /// </summary>
    /// <param name="accessKey">The access key (username) used to authenticate requests.</param>
    /// <param name="secretKey">The secret key (password) used to sign requests.</param>
    /// <param name="sessionToken">An optional temporary session token for STS-based credentials.</param>
    /// <returns>The current <see cref="IObstorBuilder"/> instance to allow further chaining.</returns>
    IObstorBuilder WithStaticCredentials(string accessKey, string secretKey, string? sessionToken = null);

    /// <summary>
    /// Configures the Obstor client to read credentials from the <c>OBSTOR_ROOT_USER</c> and
    /// <c>OBSTOR_ROOT_PASSWORD</c> environment variables at runtime.
    /// </summary>
    /// <returns>The current <see cref="IObstorBuilder"/> instance to allow further chaining.</returns>
    IObstorBuilder WithEnvironmentCredentials();
}

/// <summary>
/// Provides extension methods on <see cref="IServiceCollection"/> for registering the Obstor client
/// and its dependencies into an ASP.NET Core or generic-host dependency injection container.
/// </summary>
public static class ServiceCollectionServiceExtensions
{
    private sealed class ObstorBuilder : IObstorBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public ObstorBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IObstorBuilder WithStaticCredentials(Action<StaticCredentialsOptions>? configure = null)
        {
            if (configure != null)
                _serviceCollection.Configure(configure);
            _serviceCollection.AddSingleton<ICredentialsProvider, StaticCredentialsProvider>();
            return this;
        }

        public IObstorBuilder WithStaticCredentials(string accessKey, string secretKey, string? sessionToken = null)
            => WithStaticCredentials(opts =>
            {
                opts.AccessKey = accessKey;
                opts.SecretKey = secretKey;
                opts.SessionToken = sessionToken;
            });

        public IObstorBuilder WithEnvironmentCredentials()
        {
            _serviceCollection.AddSingleton<ICredentialsProvider, EnvironmentCredentialsProvider>();
            return this;
        }
    }

    /// <summary>
    /// Registers the Obstor client services — including an <see cref="IObstorClient"/>,
    /// <see cref="IRequestAuthenticator"/>, and the underlying named <see cref="System.Net.Http.HttpClient"/> with
    /// a Polly retry policy — into the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">
    /// An optional delegate used to configure <see cref="Obstor.ClientOptions"/> (e.g. the endpoint URL).
    /// When <see langword="null"/>, the endpoint must be configured separately.
    /// </param>
    /// <param name="lifetime">
    /// The <see cref="ServiceLifetime"/> of the <see cref="IObstorClient"/> and <see cref="IRequestAuthenticator"/>
    /// registrations. Defaults to <see cref="ServiceLifetime.Singleton"/>.
    /// </param>
    /// <returns>An <see cref="IObstorBuilder"/> that allows further configuration of the credential provider.</returns>
    public static IObstorBuilder AddObstor(
        this IServiceCollection services,
        Action<ClientOptions>? configure = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var httpBuilder =services.AddHttpClient("Obstor");
        httpBuilder.AddStandardResilienceHandler();
        services.TryAddSingleton<ITimeProvider, DefaultTimeProvider>();
        services.TryAdd(new ServiceDescriptor(typeof(IRequestAuthenticator), typeof(V4RequestAuthenticator), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IObstorClient), typeof(ObstorClient), lifetime));
        if (configure != null)
            services.Configure(configure);
        return new ObstorBuilder(services);
    }

    /// <summary>
    /// Registers the Obstor client services and sets the server endpoint from the supplied <see cref="Uri"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="endPoint">The base URI of the Obstor or S3-compatible server (e.g. <c>https://obstor.example.com</c>).</param>
    /// <returns>An <see cref="IObstorBuilder"/> that allows further configuration of the credential provider.</returns>
    public static IObstorBuilder AddObstor(this IServiceCollection services, Uri endPoint)
        => services.AddObstor(opts => opts.EndPoint = endPoint);

    /// <summary>
    /// Registers the Obstor client services and sets the server endpoint from the supplied URI string.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="endPoint">The base URI string of the Obstor or S3-compatible server (e.g. <c>"https://obstor.example.com"</c>).</param>
    /// <returns>An <see cref="IObstorBuilder"/> that allows further configuration of the credential provider.</returns>
    public static IObstorBuilder AddObstor(this IServiceCollection services, string endPoint)
        => services.AddObstor(opts => opts.EndPoint = new Uri(endPoint));
}