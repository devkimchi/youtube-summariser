namespace YouTubeSummariser.WebApp.Wasm.Configurations;

/// <summary>
/// This represents the settings entity for the prompt.
/// </summary>
public class PromptSettings
{
    /// <summary>
    /// Gets the name of the configuration.
    /// </summary>
    public const string Name = "Prompt";

    /// <summary>
    /// Gets or sets the system prompt.
    /// </summary>
    public virtual string? System { get; set; } = "You are the expert of summarising long contents. You are going to summarise the following YouTube video transcript in 5 bullet point items.";

    /// <summary>
    /// Gets or sets the maximum number of tokens to use for completion.
    /// </summary>
    public virtual int? MaxTokens { get; set; } = 3000;

    /// <summary>
    /// Gets or sets the temperature of the completion.
    /// </summary>
    public virtual float? Temperature { get; set; } = 0.7f;
}
