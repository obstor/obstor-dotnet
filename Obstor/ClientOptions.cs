namespace Obstor;

/// <summary>
/// Configuration options for the Obstor client, including endpoint, region, and HTTP client settings.
/// </summary>
public class ClientOptions
{
    /// <summary>
    /// Gets or sets the URI of the Obstor or S3-compatible endpoint to connect to.
    /// </summary>
    public required Uri EndPoint { get; set; }

    /// <summary>
    /// Gets or sets the AWS region used for request signing. Defaults to <c>us-east-1</c>.
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Gets or sets the named <see cref="System.Net.Http.HttpClient"/> to use for outbound HTTP requests.
    /// Defaults to <c>Obstor</c>.
    /// </summary>
    public string ObstorHttpClient { get; set; } = "Obstor";
}
