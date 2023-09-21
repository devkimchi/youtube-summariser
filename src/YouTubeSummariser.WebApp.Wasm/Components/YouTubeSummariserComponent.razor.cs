using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using YouTubeSummariser.WebApp.Wasm.Services;

namespace YouTubeSummariser.WebApp.Wasm.Components;

public partial class YouTubeSummariserComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the <see cref="IOpenAIService"/> instance.
    /// </summary>
    [Inject]
    protected IOpenAIService? OpenAI { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSRuntime"/> instance.
    /// </summary>
    [Inject]
    protected IJSRuntime Jsr { get; set; }

    /// <summary>
    /// Gets or sets the YouTube link URL.
    /// </summary>
    protected string? YouTubeLinkUrl { get; set; }// = "https://www.youtube.com/live/47CZqb53nCM?si=QOR3XVjcUzZSSdqX";

    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    protected string? Summary { get; set; }

    /// <summary>
    /// Handles the event when the "Summarise!" button is clicked.
    /// </summary>
    /// <param name="ev"><see cref="MouseEventArgs"/> instance.</param>
    protected async Task CompleteAsync(MouseEventArgs ev)
    {
        var caption = await this.Jsr.InvokeAsync<string>("YouTube.downloadYouTubeCaptions", this.YouTubeLinkUrl, "en");
        this.Summary = await this.OpenAI.GetCompletionsAsync(caption);
    }

    /// <summary>
    /// Handles the event when the "Clear!" button is clicked.
    /// </summary>
    /// <param name="ev"><see cref="MouseEventArgs"/> instance.</param>
    protected async Task ClearAsync(MouseEventArgs ev)
    {
        this.YouTubeLinkUrl = default;
        this.Summary = default;

        await Task.CompletedTask;
    }
}
