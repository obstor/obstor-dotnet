using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Obstor.CredentialProviders;
using Obstor.Implementation;

namespace Obstor;

/// <summary>
/// Fluent builder for constructing an <see cref="IObstorClient"/> instance without a dependency
/// injection container. Configure the endpoint, region, and credentials using the
/// <c>With*</c> methods, then call <see cref="Build"/> to create the client.
/// </summary>
/// <example>
/// <code>
/// var client = new ObstorClientBuilder("https://obstor.example.com")
///     .WithStaticCredentials("accessKey", "secretKey")
///     .Build();
/// </code>
/// </example>
public sealed class ObstorClientBuilder
{
    /// <summary>
    /// Gets the URI of the Obstor or S3-compatible endpoint.
    /// </summary>
    public Uri EndPoint { get; }

    /// <summary>
    /// Gets the AWS region used for request signing. Defaults to <c>us-east-1</c>.
    /// Can be overridden with <see cref="WithRegion"/>.
    /// </summary>
    public string Region { get; private set; } = "us-east-1";

    /// <summary>
    /// Gets the credentials provider that will be used to authenticate requests.
    /// Set via one of the <c>With*Credentials*</c> methods or <see cref="WithCredentialsProvider"/>.
    /// </summary>
    public ICredentialsProvider? CredentialsProvider { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ObstorClientBuilder"/> with the specified endpoint URI string.
    /// </summary>
    /// <param name="endPoint">The endpoint URL of the Obstor or S3-compatible service (e.g., <c>https://obstor.example.com</c>).</param>
    public ObstorClientBuilder(string endPoint) : this(new Uri(endPoint))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ObstorClientBuilder"/> with the specified endpoint URI.
    /// </summary>
    /// <param name="endPoint">The <see cref="Uri"/> of the Obstor or S3-compatible service endpoint.</param>
    public ObstorClientBuilder(Uri endPoint)
    {
        EndPoint = endPoint;
    }

    /// <summary>
    /// Builds and returns a configured <see cref="IObstorClient"/> instance using the current
    /// builder settings.
    /// </summary>
    /// <returns>A fully configured <see cref="IObstorClient"/> ready for use.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no credentials provider has been configured. Call one of the
    /// <c>With*Credentials*</c> methods before calling <see cref="Build"/>.
    /// </exception>
    public IObstorClient Build()
    {
        if (CredentialsProvider == null)
            throw new InvalidOperationException("No credentials specified");

        var clientOptions = Options.Create(new ClientOptions
        {
            EndPoint = EndPoint,
            Region = Region,
        });
        var timeProvider = new DefaultTimeProvider();
        var authLogger = NullLoggerFactory.Instance.CreateLogger<V4RequestAuthenticator>();
        var authenticator = new V4RequestAuthenticator(CredentialsProvider, timeProvider, authLogger);
        var httpClientFactory = new HttpClientFactory();
        var obstorLogger = NullLoggerFactory.Instance.CreateLogger<ObstorClient>();
        return new ObstorClient(clientOptions, timeProvider, authenticator, httpClientFactory, obstorLogger);
    }

    /// <summary>
    /// Sets the AWS region used for request signing and returns the builder for chaining.
    /// </summary>
    /// <param name="region">The AWS region identifier (e.g., <c>eu-west-1</c>).</param>
    /// <returns>The current <see cref="ObstorClientBuilder"/> instance for fluent chaining.</returns>
    public ObstorClientBuilder WithRegion(string region)
    {
        Region = region;
        return this;
    }

    /// <summary>
    /// Sets a custom <see cref="ICredentialsProvider"/> and returns the builder for chaining.
    /// Use this overload to supply a fully custom or STS-based credentials provider.
    /// </summary>
    /// <param name="credentialsProvider">The credentials provider to use for authenticating requests.</param>
    /// <returns>The current <see cref="ObstorClientBuilder"/> instance for fluent chaining.</returns>
    public ObstorClientBuilder WithCredentialsProvider(ICredentialsProvider credentialsProvider)
    {
        CredentialsProvider = credentialsProvider;
        return this;
    }

    /// <summary>
    /// Configures the client to authenticate using a fixed access key, secret key, and optional
    /// session token, then returns the builder for chaining.
    /// </summary>
    /// <param name="accessKey">The access key ID.</param>
    /// <param name="secretKey">The secret access key.</param>
    /// <param name="sessionToken">
    /// An optional temporary session token (e.g., from STS AssumeRole). Pass <see langword="null"/>
    /// for long-term credentials.
    /// </param>
    /// <returns>The current <see cref="ObstorClientBuilder"/> instance for fluent chaining.</returns>
    public ObstorClientBuilder WithStaticCredentials(string accessKey, string secretKey, string? sessionToken = null)
    {
        var credentialOptions = Options.Create(new StaticCredentialsOptions
        {
            AccessKey = accessKey,
            SecretKey = secretKey,
            SessionToken = sessionToken,
        });
        return WithCredentialsProvider(new StaticCredentialsProvider(credentialOptions));
    }

    /// <summary>
    /// Configures the client to read credentials from the <c>OBSTOR_ROOT_USER</c> and
    /// <c>OBSTOR_ROOT_PASSWORD</c> environment variables, then returns the builder for chaining.
    /// </summary>
    /// <returns>The current <see cref="ObstorClientBuilder"/> instance for fluent chaining.</returns>
    public ObstorClientBuilder WithEnvironmentCredentials()
    {
        return WithCredentialsProvider(new EnvironmentCredentialsProvider());
    }

    private sealed class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new();
    }
}
