namespace YouTubeSummariser.WebApp.Extensions;

/// <summary>
/// This represents the extension entity for the <see cref="HttpRequest"/> class.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the base URL.
    /// </summary>
    /// <param name="req"><see cref="HttpRequest"/> instance.</param>
    /// <returns>Returns the base URL.</returns>
    public static string? BaseUrl(this HttpRequest req)
    {
        if (req == null)
        {
            return null;
        }

        // For default port number:
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.Uri/src/System/UriBuilder.cs#L357-L365
        var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
        if (uriBuilder.Uri.IsDefaultPort)
        {
            uriBuilder.Port = -1;
        }

        return uriBuilder.Uri.AbsoluteUri;
    }
}
