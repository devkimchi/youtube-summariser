namespace YouTubeSummariser.ApiApp.Models;

/// <summary>
/// This represents the request model entity containing YouTube video details.
/// </summary>
public class SummariseRequestModel
{
    /// <summary>
    /// Gets or sets the YouTube video URL.
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// Gets or sets the language code.
    /// </summary>
    public string? LanguageCode { get; set; }
}
