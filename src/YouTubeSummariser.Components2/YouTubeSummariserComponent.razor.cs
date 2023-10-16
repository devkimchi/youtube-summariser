using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using YouTubeSummariser.Components.Facade;

namespace YouTubeSummariser.Components;

public partial class YouTubeSummariserComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the <see cref="YouTubeSummariserClient"/> instance.
    /// </summary>
    [Inject]
    protected YouTubeSummariserClient YouTube { get; set; }

    [Inject]
    protected IJSRuntime JSR { get; set; }

    //[Inject]
    //protected IProgressBarJsInterop ProgressBar { get; set; }

    /// <summary>
    /// Gets or sets the YouTube link URL.
    /// </summary>
    protected string? YouTubeLinkUrl { get; set; }// = "https://www.youtube.com/live/47CZqb53nCM?si=QOR3XVjcUzZSSdqX";

    /// <summary>
    /// Gets or sets the video language code.
    /// </summary>
    protected string? VideoLanguageCode { get; set; } = "en";

    /// <summary>
    /// Gets or sets the summary language code.
    /// </summary>
    protected string? SummaryLanguageCode { get; set; } = "en";

    /// <summary>
    /// Gets or sets a value indicating whether the summary has been completed or not.
    /// </summary>
    protected bool SummaryCompleted { get; set; }

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
        if (string.IsNullOrWhiteSpace(this.YouTubeLinkUrl))
        {
            return;
        }

        this.Summary = default;
        this.SummaryCompleted = false;
        //await this.JSR.InvokeVoidAsync("YouTube.RenderProgressBar");
        //await this.ProgressBar.RenderAsync();

        var request = new SummariseRequestModel
        {
            VideoUrl = this.YouTubeLinkUrl,
            VideoLanguageCode = this.VideoLanguageCode,
            SummaryLanguageCode = this.SummaryLanguageCode,
        };

        var response = default(string);
        try
        {
            response = await this.YouTube.SummariseAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            response = ex.Message;
        }

        this.Summary = response;
        this.SummaryCompleted = true;
    }

    /// <summary>
    /// Handles the event when the "Clear!" button is clicked.
    /// </summary>
    /// <param name="ev"><see cref="MouseEventArgs"/> instance.</param>
    protected async Task ClearAsync(MouseEventArgs ev)
    {
        this.YouTubeLinkUrl = default;
        this.VideoLanguageCode = "en";
        this.SummaryLanguageCode = "en";
        this.Summary = default;

        await Task.CompletedTask;
    }
}
