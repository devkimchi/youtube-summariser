namespace YouTubeSummariser.Services.Models;

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
    /// Gets or sets the video language code.
    /// </summary>
    public string? VideoLanguageCode { get; set; }

    /// <summary>
    /// Gets or sets the summary language code.
    /// </summary>
    public string? SummaryLanguageCode { get; set; }
}
