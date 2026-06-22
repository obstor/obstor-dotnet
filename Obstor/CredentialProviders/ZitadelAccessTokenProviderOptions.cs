namespace Obstor.CredentialProviders;

/// <summary>
/// Configuration options for <see cref="ZitadelAccessTokenProvider"/>.
/// Specifies the Zitadel server connection details used to obtain a client credentials token.
/// </summary>
public class ZitadelAccessTokenProviderOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Zitadel server (e.g., <c>https://zitadel.example.com</c>).
    /// This value is required.
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the name of the Zitadel realm in which the client is registered.
    /// This value is required.
    /// </summary>
    public required string Realm { get; set; }

    /// <summary>
    /// Gets or sets the client ID (name) registered in the Zitadel realm.
    /// This value is required.
    /// </summary>
    public required string ClientName { get; set; }

    /// <summary>
    /// Gets or sets the client secret associated with the registered client.
    /// This value is required.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the named <see cref="System.Net.Http.HttpClient"/> to create via
    /// <see cref="System.Net.Http.IHttpClientFactory"/> when calling the token endpoint.
    /// Defaults to <c>"Keyclock"</c>.
    /// </summary>
    public string HttpClientName { get; set; } = "Keyclock";
}
